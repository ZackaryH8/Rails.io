using Discord;
using Discord.WebSocket;
using RailsIO.Interfaces;
using ParkSquare.RealTimeTrains;
using RailsIO.Utilities;
using RailsIO.Extensions;

namespace RailsIO.Commands.Slash
{
    class NextArrival : ISlashCommand
    {
        public string Name => "nextarrival";
        public string Description => "Get the next arrival for the specified station.";

        public SlashCommandOptionBuilder[] Options => new SlashCommandOptionBuilder[] {
            new()
            {
                Name = "stationcode",
                Description = "The station to retrieve arrivals for eg. PBO",
                Type = ApplicationCommandOptionType.String,
                IsRequired = true,
                
                // TODO - Uncomment when added to .NET API
                //MinLength = 3,
                //MaxLength = 3
            },
        };



        public async Task Execute(SocketSlashCommand slashCommand)
        {
            // Slash command options
            var stationCode = slashCommand.Data.Options.FirstOrDefault(opt => opt.Name == "stationcode")!.Value
                                                       .ToString()!.ToUpper();
            

            // Get API Data
            var client = Program.Container.GetInstance<RealTimeTrainsClient>();

            // Get departures and find the next one
            var departures = client.GetArrivals(stationCode);

            // Check if there is any services
            if (!departures.Services.Any())
            {
                await slashCommand.RespondAsync($"No departures found for {stationCode}");
                return;
            }
            
            // Get the next departure
            var nextArrival = departures.Services.ToArray()[0];


            var serviceDetail = client.GetService(nextArrival.ServiceUid, DateFilterUtility.Now);

            // Sort data for the embed
            var arrivalTime = nextArrival.LocationDetail.RealtimeArrival ?? nextArrival.LocationDetail.PublicTimetableBookedArrival;
            var headCode = nextArrival.TrainIdentity;
            var origin = nextArrival.LocationDetail.Origin[0].Description;
            var destination = nextArrival.LocationDetail.Destination[0].Description;
            var lateness = nextArrival.LocationDetail.RealtimePublicTimetableArrivalLateness;

            // This checks if we can display calling points or not
            var callingPoints = serviceDetail.ServiceUid == null ? "No calling points..." : ListUtility.Commaize(serviceDetail.Locations.Select(x => x.Description));

            // Create Embed
            var embed = new EmbedBuilder
            {
                Title = $"{arrivalTime.AsDateTime:HH:mm} {origin} to {destination}",
                Color = ColorUtility.GetAtocColor(nextArrival.AtocCode),
                Description = "**Calling Points**\n" + callingPoints
            };

            // Embed Fields
            embed.AddField("Headcode", headCode, true);
            embed.AddField("Operator", nextArrival.AtocName, false);

            // If the train is late, add the lateness to the embed
            // Otherwise show on time 
            var nextArrivalRealTime = nextArrival.LocationDetail.RealtimeArrival!.AsDateTime;
            var nextArrivaBookedTime = nextArrival.LocationDetail.PublicTimetableBookedArrival.AsDateTime;

            if (nextArrivalRealTime > nextArrivaBookedTime)
            {
                var latness = nextArrivalRealTime - nextArrivaBookedTime;
                embed.AddField("Status", $"{latness:mm} min(s) late", true);
            }
            else
            {
                embed.AddField("Status", "On Time", true);
            }
           

            embed.AddField("Platform", nextArrival.LocationDetail.Platform ?? "Unknown", true);
            embed.AddField(EmbedUtility.CreateBlankField(true));
            embed.AddField("Origin", origin, true);
            embed.AddField("Destination", destination, true);
            embed.AddField(EmbedUtility.CreateBlankField(true));

            // Send the embed
            await slashCommand.RespondAsync(embed: embed.Build());
        }
    }
}

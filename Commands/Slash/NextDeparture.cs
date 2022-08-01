using Discord;
using Discord.WebSocket;
using RailsIO.Interfaces;
using ParkSquare.RealTimeTrains;
using RailsIO.Utilities;
using SimpleInjector;
using RailsIO.Extensions;
using System.Text.RegularExpressions;

namespace RailsIO.Commands.Slash
{
    class NextDeparture : ISlashCommand
    {
        public string Name => "nextdeparture";
        public string Description => "Get the next departure for the specified station.";

        public SlashCommandOptionBuilder[] Options => new SlashCommandOptionBuilder[] {
            new()
            {
                Name = "stationcode",
                Description = "The station to retrieve departures for eg. PBO",
                Type = ApplicationCommandOptionType.String,
                IsRequired = true,
                
                // TODO - Uncomment when added to .NET API
                //MinLength = 3,
                //MaxLength = 3
            },
        };



        public async Task Execute(SocketSlashCommand slashCommand)
        {
            var stationCode = slashCommand.Data.Options.FirstOrDefault(opt => opt.Name == "stationcode")!.Value
                                                       .ToString()!.ToUpper();

            // Get API Data
            var client = Program.Container.GetInstance<RealTimeTrainsClient>();

            // Get departures and find the next one
            var departures = client.GetDepartures(stationCode);

            // Check if there is any services
            if (!departures.Services.Any())
            {
                await slashCommand.RespondAsync($"No departures found for {stationCode}");
                return;
            }

            // Get the next departure
            var nextDeparture = departures.Services.ToArray()[0];
            var serviceDetail = client.GetService(nextDeparture.ServiceUid, DateFilterUtility.Now);

            // Sort data for the embed
            var departureTime = nextDeparture.LocationDetail.RealtimeDeparture ?? nextDeparture.LocationDetail.PublicTimetableBookedDeparture;
            var headCode = nextDeparture.TrainIdentity;
            var origin = nextDeparture.LocationDetail.Origin[0].Description;
            var destination = nextDeparture.LocationDetail.Destination[0].Description;
            var lateness = nextDeparture.LocationDetail.RealtimePublicTimetableArrivalLateness;

            // This checks if we can display calling points or not
            var callingPoints = serviceDetail.ServiceUid == null ? "No calling points..." : ListUtility.Commaize(serviceDetail.Locations.Select(x => x.Description));

            // Create Embed
            var embed = new EmbedBuilder
            {
                Title = $"{departureTime.AsDateTime:HH:mm} {origin} to {destination}",
                Color = ColorUtility.GetAtocColor(nextDeparture.AtocCode),
                Description = "**Calling Points**\n" + callingPoints
            };

            // Embed Fields
            embed.AddField("Headcode", headCode, true);
            embed.AddField("Operator", nextDeparture.AtocName, false);

            // If the train is late, add the lateness to the embed
            // Otherwise show on time 
            var nextDepartureRealTime = nextDeparture.LocationDetail.RealtimeDeparture?.AsDateTime;
            var nextDepartureBookedTime = nextDeparture.LocationDetail.PublicTimetableBookedDeparture.AsDateTime;

            if (nextDepartureRealTime > nextDepartureBookedTime)
            {
                var latness = nextDeparture.LocationDetail.RealtimeDeparture!.AsDateTime - nextDeparture.LocationDetail.PublicTimetableBookedDeparture.AsDateTime;
                embed.AddField("Status", $"{latness:mm} min(s) late", true);
            }
            else
            {
                embed.AddField("Status", "On Time", true);
            }
           

            embed.AddField("Platform", nextDeparture.LocationDetail.Platform ?? "Unknown", true);
            embed.AddField(EmbedUtility.CreateBlankField(true));

            embed.AddField("Origin", origin, true);
            embed.AddField("Destination", destination, true);
            embed.AddField(EmbedUtility.CreateBlankField(true));



            await slashCommand.RespondAsync(embed: embed.Build());

        }
    }
}

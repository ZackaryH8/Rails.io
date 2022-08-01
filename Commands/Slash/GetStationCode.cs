using Discord;
using Discord.WebSocket;
using RailsIO.Interfaces;
using RailsIO.Utilities;

namespace RailsIO.Commands.Slash
{
    public struct StationRecord
    {
        public string Name { get; set; }
        public string Code { get; set; }
    }
    
    class GetStationCode : ISlashCommand
    {
        public string Name => "getstationcode";
        public string Description => "Get the CRS code of the specified station.";

        public SlashCommandOptionBuilder[] Options => new SlashCommandOptionBuilder[] {
            new()
            {
                Name = "station",
                Description = "The station to retrieve the CRS code for eg. Peterborough",
                Type = ApplicationCommandOptionType.String,
                IsRequired = true,
                
                // TODO - Uncomment when added to .NET API
                //MinLength = 3,
                //MaxLength = 3
            },
        };


        public async Task Execute(SocketSlashCommand slashCommand)
        {
            // Command Arguments
            var stationArg = slashCommand.Data.Options.FirstOrDefault(opt => opt.Name == "station")!.Value
                                                       .ToString()!.ToUpper();

            // Error checking
            if(stationArg.Length <= 3)
            {
                await slashCommand.RespondAsync("Please use a longer search term! (greater than 3 characters).", ephemeral: true);
                return;
            }

            // Read CSV file and return a StationRecord with Name and Code
            var foundStations = CRSUtility.Records.Where(station => station.Name.ToUpper().Contains(stationArg));

            // If no stations are found, return error message
            if (!foundStations.Any())
            {
                await slashCommand.RespondAsync($"**Found no results for search term: {stationArg}**", ephemeral: true);
                return;
            }

            // Return useful results
            await slashCommand.RespondAsync($"**Found {foundStations.Count()} results for search term: {stationArg}**\n" +
                $"```yaml\n{string.Join("\n", foundStations.Select(station => $"{station.Code} - {station.Name}"))}```");
        }
    }
}
    
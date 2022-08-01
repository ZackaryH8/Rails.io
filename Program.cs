using System;
using Discord;
using Discord.Net;
using Discord.WebSocket;
using RailsIO.Interfaces;
using RailsIO.RTT;
using RailsIO.Utilities;
using Newtonsoft.Json;
using ParkSquare.RealTimeTrains;
using SimpleInjector;

namespace RailsIO
{
    internal class Program
    {
        private static readonly DiscordSocketClient _client = new();
        public static readonly Container Container = new();

        private static Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }

        private async Task SlashCommandHandler(SocketSlashCommand slashCommand)
        {
            // Search for command by name
            var foundCommand = Container.GetAllInstances<ISlashCommand>().Where(cmd => cmd.Name == slashCommand.Data.Name).First();
            
            // If command is found, then execute it
            if (foundCommand != null)
            {
                await foundCommand.Execute(slashCommand);
            }   
        }

        public static Task Main(string[] args) => new Program().MainAsync();

        public async Task MainAsync()
        {

            _client.Log += Log;

            var token = "";

            // Some alternative options would be to keep your token in an Environment Variable or a standalone file.
            //var token = Environment.GetEnvironmentVariable("NameOfYourEnvironmentVariable");
            // var token = File.ReadAllText("token.txt");
            // var token = JsonConvert.DeserializeObject<AConfigurationClass>(File.ReadAllText("config.json")).Token;

            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();

            _client.Ready += Client_Ready;
            _client.SlashCommandExecuted += SlashCommandHandler;

            await Task.Delay(-1);
        }

        public async Task Client_Ready()
        {
            // Register RTT Client Singleton
            Container.Register(() => new RealTimeTrainsClient(new HttpClient(), new ClientConfig()), Lifestyle.Singleton);

            // Load CRS Records
            CRSUtility.LoadCrsRecords();

            // Register Commands
            Container.Collection.Register<ISlashCommand>(typeof(ISlashCommand).Assembly);
            
            foreach (var command in Container.GetAllInstances<ISlashCommand>())
            {

                var guildCommand = new SlashCommandBuilder();
                    
                guildCommand.WithName(command.Name);
                guildCommand.WithDescription(command.Description);
                guildCommand.AddOptions(command.Options);
                
                try
                {
                    await _client.CreateGlobalApplicationCommandAsync(guildCommand.Build());
                }
                catch (HttpException exception)
                {
                    var json = JsonConvert.SerializeObject(exception.Errors, Formatting.Indented);
                    Console.WriteLine(json);
                }
            }

            await Log(new LogMessage(LogSeverity.Info,
                "Commands",
                $"{Container.GetAllInstances<ISlashCommand>().Count()} command(s) were registered!"
            ));
        }
    }
}
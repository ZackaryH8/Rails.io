using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailsIO.Interfaces
{

    public interface ISlashCommand
    {
        string Name { get; }

        string Description { get; }

        SlashCommandOptionBuilder[]? Options { get; }

        Task Execute(SocketSlashCommand slashCommand);
    }
}

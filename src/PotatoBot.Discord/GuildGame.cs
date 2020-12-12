using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using PotatoBot.Domain.Models;

namespace PotatoBot.Discord
{
    public class GuildGame
    {
        public Domain.Models.Game CurrentGame { get; set; }
        public ulong GuildId { get; set; }
        public Dictionary<ulong, Player> Players = new Dictionary<ulong, Player>();
        public bool IsThereAPotato => CurrentGame != null;
        public Func<string, Task<IUserMessage>> ReplyAsync = null;
        
        public bool Tick()
        {
            if (CurrentGame == null) return false;
            CurrentGame.GamePotato.CoolBy(5);
            if (CurrentGame.IsOver) return true;
            return false;
        }
    }
}
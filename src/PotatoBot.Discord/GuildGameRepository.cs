using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace PotatoBot.Discord
{
    public class GuildGameRepository
    {
        private ConcurrentDictionary<ulong, GuildGame> _runningGames = new ConcurrentDictionary<ulong, GuildGame>();

        public GuildGame GetGuildGameByGuildId(ulong guildId)
        {
            if (!_runningGames.ContainsKey(guildId))
                return null;

            return _runningGames[guildId];
        }

        public void StoreGuildGame(GuildGame game)
        {
            RemoveGuildGame(game);
            _runningGames.TryAdd(game.GuildId, game);
        }

        public void RemoveGuildGame(GuildGame game)
        {
            RemoveGuildGameByGuildId(game.GuildId);
        }

        public void RemoveGuildGameByGuildId(ulong guildId)
        {
            if (_runningGames.ContainsKey(guildId))
            {
                GuildGame guildGame;
                _runningGames.Remove(guildId, out guildGame);
            }
        }

        public IEnumerable<GuildGame> GetAllGuildGames()
        {
            return _runningGames.Select(x => x.Value);
        }
    }
}
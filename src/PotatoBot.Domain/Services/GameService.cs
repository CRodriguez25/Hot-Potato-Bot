using System.Collections.Generic;
using PotatoBot.Domain.Models;

namespace PotatoBot.Domain.Services
{
    public class GameService
    {
        public Game StartGame(Player originalPotatoBaker)
        {
            var game = new Game(originalPotatoBaker);
            return game;
        }
    }
}
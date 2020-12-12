using System;
using System.Linq;
using PotatoBot.Domain.Models;

namespace PotatoBot.Domain.Services
{
    public class PotatoTossService
    {
        public void TossPotato(Game game, Player playerOne, Player playerTwo)
        {
            if (!game.Players.Any(x => x.Id == playerTwo.Id))
            {
                game.AddPlayer(playerTwo);
            }

            var rnd = new Random();
            playerOne.HeldPotato.CoolBy(rnd.Next(5, 10));
            playerTwo.HeldPotato = playerOne.HeldPotato;
            playerOne.HeldPotato = null;
        }
    }
}
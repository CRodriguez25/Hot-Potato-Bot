using System;
using System.Collections.Generic;
using System.Linq;

namespace PotatoBot.Domain.Models
{
    public class Game
    {
        public Guid Id { get; init; }
        public ISet<Player> Players { get; set; } = new HashSet<Player>();
        public PoisonedPotato GamePotato { get; set; }
        public bool IsOver => GamePotato.Heat <= 0;

        public Player PotatoHolder => Players.First(x => x.IsHoldingPotato);

        public Game(Player baker)
        {
            Id = new Guid();
            GamePotato = new PoisonedPotato(this);
            Players.Add(baker);
            baker.HeldPotato = GamePotato;
        }

        public void AddPlayer(Player playerTwo)
        {
            Players.Add(playerTwo);
        }
    }
}
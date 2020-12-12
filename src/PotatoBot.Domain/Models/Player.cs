using System;

namespace PotatoBot.Domain.Models
{
    public class Player
    {
        public Game Game { get; set; }
        public Guid Id { get; set; }
        public PoisonedPotato HeldPotato { get; set; }
        public bool IsHoldingPotato => HeldPotato != null;
        public Player()
        {
            Id = Guid.NewGuid();
        }
    }
}
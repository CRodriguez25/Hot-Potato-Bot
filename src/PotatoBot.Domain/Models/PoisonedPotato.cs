using System;

namespace PotatoBot.Domain.Models
{
    public class PoisonedPotato 
    {
        public int Heat { get; private set; } = new Random().Next(100, 200);
        public bool CanEat => Heat <= 0;
        public Game Game { get; }
        public PoisonedPotato(Game game)
        {
            Game = game;
        }

        public void CoolBy(int amount) 
        {
            Heat -= amount;        
        }
    }
}
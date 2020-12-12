using Xunit;
using FluentAssertions;
using PotatoBot.Domain.Models;
using PotatoBot.Domain.Services;

namespace PotatoBot.Domain.Tests.Services
{
    public class PotatoTossServiceTests
    {
        private PotatoTossService _potatoTossService = new PotatoTossService();

        [Fact]
        public void WhenPlayerStartsGame_PlayerIsHoldingPotato()
        {
            var playerOne = new Player();
            var game = new Game(playerOne);
            playerOne.IsHoldingPotato.Should().BeTrue();
        }

        [Fact]
        public void WhenInGamePlayerTossesToAnotherPlayerNotInGame_OtherPlayerAdded()
        {
            var playerOne = new Player();
            var game = new Game(playerOne);
            var playerTwo = new Player();

            game.Players.Should().Contain(playerOne);
            game.Players.Should().NotContain(playerTwo);

            _potatoTossService.TossPotato(game, playerOne, playerTwo);
            game.Players.Should().Contain(playerTwo);
        }

        [Fact]
        public void WhenPlayerTossesPotato_PotatoShouldCool()
        {
            var playerOne = new Player();
            var game = new Game(playerOne);
            var playerTwo = new Player();
            var initialHeat = game.GamePotato.Heat;
            _potatoTossService.TossPotato(game, playerOne, playerTwo);
            game.GamePotato.Heat.Should().BeLessThan(initialHeat);
        }

        [Fact]
        public void WhenPlayerTossesPotato_OtherPlayerShouldBeHoldingPotato()
        {
            var playerOne = new Player();
            var game = new Game(playerOne);
            var playerTwo = new Player();
            game.AddPlayer(playerTwo);
            playerOne.IsHoldingPotato.Should().BeTrue();
            _potatoTossService.TossPotato(game, playerOne, playerTwo);
            playerOne.IsHoldingPotato.Should().BeFalse();
            playerTwo.IsHoldingPotato.Should().BeTrue();
        }

        [Fact]
        public void WhenPotatoReachesMinHeat_GameShouldBeOver()
        {
            var playerOne = new Player();
            var game = new Game(playerOne);
            var playerTwo = new Player();
            game.AddPlayer(playerTwo);
            game.GamePotato.CoolBy(game.GamePotato.Heat - 1);
            game.IsOver.Should().BeFalse();
            _potatoTossService.TossPotato(game, playerOne, playerTwo);
            game.IsOver.Should().BeTrue();
        }
    }
}
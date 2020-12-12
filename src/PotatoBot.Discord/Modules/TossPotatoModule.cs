using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using PotatoBot.Domain.Models;
using PotatoBot.Domain.Services;

namespace PotatoBot.Discord.Modules
{
    public class TossPotatoModule : ModuleBase<SocketCommandContext>
    {
        private PotatoTossService _potatoTossService;
        private GuildGameRepository _guildGameRepository;

        public TossPotatoModule(
            PotatoTossService potatoTossService, 
            GuildGameRepository guildGameRepository)
        {
            _potatoTossService = potatoTossService;
            _guildGameRepository = guildGameRepository;
        }

        [Command("toss")]
        public async Task Toss(string targetPlayerMention)
        {
            ulong targetPlayerUserId = 0;
            var isValidMention = MentionUtils.TryParseUser(targetPlayerMention, out targetPlayerUserId);
            var targetPlayer = Context.Guild.Users.FirstOrDefault(x => x.Id == targetPlayerUserId);
            if (targetPlayer == null || targetPlayer.IsBot)
            {
                await ReplyAsync("That's not a real person!");
                return;
            }

            var guildGame = _guildGameRepository.GetGuildGameByGuildId(Context.Guild.Id);
            if (guildGame == null) 
                guildGame = await CreateGameInGuild(targetPlayerMention);

            if (!guildGame.Players.Keys.Any(x => x == targetPlayer.Id))
            {
                guildGame.Players.Add(targetPlayer.Id, new Player());
            }

            var playerOne = guildGame.Players[Context.User.Id];
            var playerTwo = guildGame.Players[targetPlayer.Id];

            if (!playerOne.IsHoldingPotato)
            {
                await ReplyAsync("You don't have a :potato: to toss :(");
                return;
            }

            if (playerOne == playerTwo)
            {
                await ReplyAsync("You juggle the :potato: in your hands, achieving nothing...");
                return;
            }

            _potatoTossService.TossPotato(guildGame.CurrentGame, playerOne, playerTwo);

            if (!guildGame.CurrentGame.IsOver)
            {
                await ReplyAsync($"{targetPlayerMention} now has the :potato:, but it's too hot to eat! Toss it!");
            }
            else
            {
                _guildGameRepository.RemoveGuildGame(guildGame);
                await ReplyAsync($"Mmm... The :potato: is finally cool enough to eat! {targetPlayerMention} swallows it whole!");
                await ReplyAsync($":nauseated_face: Uh oh! The potato was rotten or something! {targetPlayerMention} vomits all over the floor! :face_vomiting:");
            }
        }

        private GuildGame CreateGameForGuild(IGuild guild, IUser user)
        {
            var player = new Player();
            var game = new Domain.Models.Game(player);
            var guildGame = new GuildGame {
                CurrentGame = game,
                GuildId = guild.Id
            };

            guildGame.Players.Add(user.Id, player);
            _guildGameRepository.StoreGuildGame(guildGame);
            return guildGame;
        }

        public async Task<GuildGame> CreateGameInGuild(string targetPlayerMention)
        {
            await ReplyAsync($"{Context.User.Mention} pulls a yummy :potato: out of the oven.");
            await ReplyAsync($":fire: OH NO! IT'S TOO HOT! {Context.User.Mention} tosses it to {targetPlayerMention} :fire:");
            var guildGame = CreateGameForGuild(Context.Guild, Context.User);
            guildGame.ReplyAsync = (msg) => ReplyAsync(msg);
            return guildGame;
        }
    }
}
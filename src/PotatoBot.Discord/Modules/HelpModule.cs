using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace PotatoBot.Discord.Modules
{
    public class HelpModule : ModuleBase<SocketCommandContext>
    {
        private const string HelpMessage = "Help? Just type `potato toss @target` to throw a potato! It couldn't be simpler!";

        [Command("help")]
        public async Task Help([Remainder]string _) 
        {
            await ReplyAsync("", false, GetHelpMessage());
        }

        [Command("help")]
        public async Task Help() 
        {
            await ReplyAsync("", false, GetHelpMessage());
        }

        private Embed GetHelpMessage()
        {
            var embedBuilder = new EmbedBuilder();
            embedBuilder
                .WithTitle("Help")
                .WithDescription(HelpMessage);
            return embedBuilder.Build();
        }                
    }
}
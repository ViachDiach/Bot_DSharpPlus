using DSharpPlus;
using DSharpPlus.SlashCommands;
using DSharpPlus.Entities;
using DSharpAPP.db;

namespace DSharpAPP.commands
{
    public partial class Program
    {
        private static async Task<DiscordEmbedBuilder> Swaper(InteractionContext ctx, string authorId, string preyId)
        {
            string [] preyInfo = SQL.PreyFound(preyId);
            string [] authorInfo = SQL.PreyFound(authorId);
            DiscordEmbedBuilder embed;
            
            if(preyInfo[3].Length != 0 && authorInfo[3].Length != 0)
            {
                SQL.AddCurse(preyInfo[1], authorInfo[3]);
                SQL.AddCurse(authorId, preyInfo[3]);

                embed = new DiscordEmbedBuilder()
                    .WithColor(DiscordColor.Orange)
                    .WithDescription($"{authorInfo[0]} и {preyInfo[0]} взялись за голандский штурвал");
            }
            else
            {
                embed = new DiscordEmbedBuilder()
                    .WithColor(DiscordColor.Red)
                    .WithDescription("Кто-то из вас не прошел /zamat + /stuffy");
            }
            return embed;
        }
        public class SwapCommands : ApplicationCommandModule
        {
            [SlashCommand("swap", "Обменяться % душноты")]
            public async Task SwapCommand(InteractionContext ctx, [Option("who", "+UserName c кем обменяться?")] string MyPrey)
            {
                string preyId = MyPrey.Replace("<", "").Replace(">", "").Replace("@", "");
                string authorId =  ctx.User.Id.ToString();

                Console.WriteLine($"{preyId} = preyId");
                Console.WriteLine($"{authorId} = authorId");

                var embed = await Swaper(ctx, authorId, preyId);

                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                        new DiscordInteractionResponseBuilder()
                            .WithContent("Ясненько")
                            .AddEmbed(embed)
                            );
            }
        }
    }
}
using DSharpPlus;
using DSharpPlus.SlashCommands;
using DSharpPlus.Entities;
using DSharpAPP.db;

namespace DSharpAPP.commands
{
    public partial class Program
    {
        public class StuffyCommands : ApplicationCommandModule
        {
            [SlashCommand("stuffy", "Ha сколько % ты душный")]
            public async Task StuffyCommand(InteractionContext ctx)
            {
                string authorName = ctx.User.Username;
                string authorId = ctx.User.Id.ToString();

                string[] dataAnswer = SQL.Checking(authorName);
                DiscordEmbedBuilder embed;

                if (dataAnswer[2].Length == 0)
                {
                    Random rnd = new Random();
				    int rate = rnd.Next(1, 101);
                    SQL.AddProcent(authorName, authorId, rate);

                    embed = new DiscordEmbedBuilder()
                            .WithTitle($"Hy и ну...")
                            .WithColor(DiscordColor.Orange)
                            .WithDescription($"Так, ты сегодня душный аж на **{rate}**%");
                }
                else
                {
                    embed = new DiscordEmbedBuilder()
                            .WithTitle($"Погодите-ка")
                            .WithColor(DiscordColor.Magenta)
                            .AddField($"Ты уже есть в списке.", "Смотри /list");
                }

                 await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, 
                    new DiscordInteractionResponseBuilder()
                        .AddEmbed(embed));
            }
        }
    }
}
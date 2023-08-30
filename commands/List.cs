using DSharpPlus;
using DSharpPlus.SlashCommands;
using DSharpPlus.Entities;
using DSharpAPP.db;

namespace DSharpAPP.commands
{
    public partial class Program
    {
        public class ListCommands : ApplicationCommandModule
        {
            [SlashCommand("list", "Узнать кто есть кто")]
            public async Task ListCommand(InteractionContext ctx)
            {
                List<string[]> dataAnswer = SQL.Select();

                DiscordEmbedBuilder embed = new DiscordEmbedBuilder();
                embed.Title = "Сейчас ты узнаешь кто есть кто";
                embed.Color = new DiscordColor(0, 255, 0); // Зеленый цвет

                embed.AddField("Имя", "(UserName)", true);
                embed.AddField("Духота", "(%)", true);
                embed.AddField("Обзывалка", "(who are U)", true);

                for (int i = 0; i < dataAnswer.Count; i++)
                {
                    embed.AddField("===========", dataAnswer[i][0], true);
                    embed.AddField("=======", dataAnswer[i][1], true);
                    embed.AddField("==========", dataAnswer[i][2], true);
                }	
                embed.Footer = new DiscordEmbedBuilder.EmbedFooter();

                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, 
                        new DiscordInteractionResponseBuilder()
                            .AddEmbed(embed));
            }
        }
    }
}
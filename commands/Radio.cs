using DSharpPlus;
using DSharpPlus.SlashCommands;
using YoutubeExplode;
using static DSharpAPP.commands.Program;

namespace DSharpAPP.commands
{
    public partial class Program
    {
        public class RadioCommands : ApplicationCommandModule
        {
            [SlashCommand("radio", "Подборка готовых плейлистов")]
            public async Task RadioPlayer(InteractionContext ctx,
                [Option("Плейлист", "Выбери что хочешь")] 
                [Choice("VirginiaSoul", "VirginiaSoul")]
                [Choice("RussianDoomer", "RussianDoomer")]
                [Choice("Punk", "Punk")]
                [Choice("Fonk", "Fonk")]
                [Choice("ClassicRock", "ClassicRock")]
                string choice
                )
            {
                string audioLink = string.Empty; // Ссылка на аудио-файл
                List<string> audioContent = new  List<string>();
                var youtubeClient = new YoutubeClient();
                MusicCommands musicCommands = new MusicCommands();

                switch (choice)
                {
                    case "VirginiaSoul":
                        audioLink = "VirginiaSoul.txt";
                        break;
                    case "RussianDoomer":
                        audioLink = "RussianDoomer.txt";
                        break;
                    case "Punk":
                        audioLink = "Punk.txt";
                        break;
                    case "Fonk":
                        audioLink = "Fonk.txt";
                        break;
                    case "ClassicRock":
                        audioLink = "ClassicRock.txt";
                        break;
                }

                audioLink = Path.Combine(Directory.GetCurrentDirectory(), "commands", "AudioData", audioLink);

                try
                {
                    audioContent = File.ReadAllLines(audioLink).ToList();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                
                await ctx.Channel.SendMessageAsync($"Аудио из {choice} добавлено в очередь");
                
                foreach (string item in audioContent)
                    await musicCommands.GetVideoTitle(ctx, youtubeClient, item);
                
                // await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent($"Аудио из {choice} добавлено в очередь. Теперь можно запустить мущыку (JOIN and\\or PLAY)"));
            }
        }   
    }
}
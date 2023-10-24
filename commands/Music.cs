using DSharpPlus;
using DSharpPlus.SlashCommands;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.VoiceNext;
using DSharpPlus.Lavalink;
using YoutubeExplode;


namespace DSharpAPP.commands
{
    public partial class Program
    {   
        public static Queue<KeyValuePair<string, string>> PlayList = new Queue<KeyValuePair<string, string>>();
    
        public class MusicCommands : ApplicationCommandModule
        {
            [SlashCommand("join", "Подключить бота к голосовому каналу")]
            public async Task Join(InteractionContext ctx)
            {
                var lava = ctx.Client.GetLavalink();
                if (!lava.ConnectedNodes.Any())
                {
                    await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                             new DiscordInteractionResponseBuilder().WithContent("Соединение Lavalink не установлено"));
                    return;
                }

                var node = lava.ConnectedNodes.Values.First();
                var voiceState = ctx.Member?.VoiceState;
                var channel = voiceState.Channel;

                if (channel.Type != ChannelType.Voice)
                {
                    await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                             new DiscordInteractionResponseBuilder().WithContent("Недопустимый голосовой канал"));
                    return;
                }

                await node.ConnectAsync(channel);
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                             new DiscordInteractionResponseBuilder().WithContent($"Бот подключен к {channel.Name}!"));
            }

            [SlashCommand("play", "Запустить проигрыватель")]
            public async Task TestPlayMusic(InteractionContext ctx, 
            [Option("link", "Ссылка на источник")] 
            [RemainingText] string link = "default")
            {
                try
                {
                    if (link == "default")
                    {
                        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                            new DiscordInteractionResponseBuilder().WithContent("Укажите ссылку на источник"));
                        return;
                    }

                    Console.WriteLine($"ССЫЛКА ПОЛУЧИНА");
                    
                    var youtubeClient = new YoutubeClient();
                    var response = await GetVideoTitle(ctx, youtubeClient, link);

                    if (response != null)
                    {
                        Console.WriteLine($"ПАРСИНГ ПОЛУЧИЛИ");
                        var embed = CreateEmbed(response);

                        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                            new DiscordInteractionResponseBuilder().AddEmbed(embed));
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            [SlashCommand("stop", "Остановить музыку и удалить очередь")]
            public async Task StopMusic(InteractionContext ctx)
            {
                string answer = "Бот не находится в голосовом канале";
                var vnext = ctx.Client.GetVoiceNext();
                var connection = vnext.GetConnection(ctx.Guild);

                if(connection != null)
                {
                    PlayList.Clear();
                    answer = "Проигрыватель выключен, очередь очищена";
                    connection.Disconnect();
                }

                var embed = CreateEmbed(answer);

                    await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                            new DiscordInteractionResponseBuilder()
                                .AddEmbed(embed));
            }


            [SlashCommand("queue", "Очередь композиций")]
            public async Task ViewQueue(InteractionContext ctx)
            {
                string answer = ""; 
                int trackNumber = 1;

                if(PlayList.Count > 0)
                {
                    foreach(var item in PlayList)
                    {
                        answer += $"{trackNumber}. {item.Key}\n";
                        trackNumber++;
                    }
                }
                else
                {
                    answer = "Очередь пуста";
                }

                var embed = CreateEmbed(answer);

                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                            new DiscordInteractionResponseBuilder()
                                .AddEmbed(embed));
            }


            private DiscordEmbed CreateEmbed(string answer)
            {
                return  new DiscordEmbedBuilder()
                    .WithColor(DiscordColor.Teal )
                    .WithDescription($"{answer}");
            }

            private async Task<string> GetVideoTitle(InteractionContext ctx, YoutubeClient youtubeClient, string link)
            {
                try
                {
                    var lava = ctx.Client.GetLavalink();
                    var nodes = lava.ConnectedNodes;
                    var node = nodes.Values.First();
                    var conn = node.GetGuildConnection(ctx.Member.VoiceState.Guild);

                    var video = await youtubeClient.Videos.GetAsync(link);
                    var title = video.Title;
                    var author = video.Author.ChannelTitle;
                    var duration = video.Duration;
                    int totalMinutes = (int)duration?.TotalMinutes;
                    int remainingSeconds = (int)duration?.TotalSeconds % 60;

                    KeyValuePair<string, string> item = new KeyValuePair<string, string>(title, link);

                    Console.WriteLine($"Добавил в очередь {title} and {link}");

                    PlayList.Enqueue(item);

                    // Если сейчас не играет ничего, вызываем метод PlaySound
                    if (conn.CurrentState.CurrentTrack == null)
                    {   
                        Console.WriteLine($"ОТПАРВЛЯЕМ НА ЗАПУСК");
                        await PlaySound(ctx);
                        return $"**Трек** __{author} - {title}__. \n**Длина**: __{totalMinutes}:{remainingSeconds:D2}__ \nзапустил";
                    }

                    Console.WriteLine($"ДОБАВЛЯЕМ В ОЧЕРЕДЬ");
                    return $"**Трек** __{author} - {title}__ добавлен в очередь";
                }
                catch (Exception)
                {
                    return null;
                }
            }


            public async Task PlaySound(InteractionContext ctx)
            {
                var lava = ctx.Client.GetLavalink();
                var nodes = lava.ConnectedNodes;
                var node = nodes.Values.First();
                var conn = node.GetGuildConnection(ctx.Member.VoiceState.Guild);

                while (PlayList.Count > 0)
                {
                    Console.WriteLine($"НАЧАЛИ ИГРАТЬ");
                    var nextTrack = PlayList.Dequeue();
                    Console.WriteLine($"На очереди {nextTrack}");

                    var loadResult = await node.Rest.GetTracksAsync(nextTrack.Value);

                    if (loadResult.Tracks.Any())
                    {
                        var track = loadResult.Tracks.First();
                        await conn.PlayAsync(track);
                    }

                    await Task.Delay(000); // Добавьте задержку в 2 секунды перед следующим треком
                }
            }
        }
    }
}


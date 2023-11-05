using DSharpPlus;
using DSharpPlus.SlashCommands;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
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
            public async Task JoinToVC(InteractionContext ctx)
            {
                var lava = ctx.Client.GetLavalink();
                if (!lava.ConnectedNodes.Any())
                {
                    await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                             new DiscordInteractionResponseBuilder().WithContent("Соединение Lavalink не установлено"));
                }

                var node = lava.ConnectedNodes.Values.First();
                var voiceState = ctx.Member?.VoiceState;
                var channel = voiceState.Channel;

                if (channel.Type != ChannelType.Voice)
                {
                    await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                             new DiscordInteractionResponseBuilder().WithContent("Недопустимый голосовой канал"));
                }

                await node.ConnectAsync(channel);
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                             new DiscordInteractionResponseBuilder().WithContent($"Бот подключен к {channel.Name}!"));
            }

            [SlashCommand("play", "Запустить проигрыватель")]
            public async Task PlayMusic(InteractionContext ctx, 
            [Option("link", "Ссылка на источник")] 
            [RemainingText] string link = "default")
            {
                try
                {
                    if (link == "default" && PlayList.Count == 0)
                    {
                        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                            new DiscordInteractionResponseBuilder().WithContent("Укажите ссылку на источник"));
                    }
                    else if(link == "default" && PlayList.Count > 0)
                    {
                        LavaLinkPlaySound(ctx);
                    }
                    
                    var youtubeClient = new YoutubeClient();
                    var response = await GetVideoTitle(ctx, youtubeClient, link);

                    if (response != null)
                    {
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

            [SlashCommand("leave", "Выгнать бота из голосового канала")]
            public async Task Leave(InteractionContext ctx)
            {
                var lava = ctx.Client.GetLavalink();
                if (!lava.ConnectedNodes.Any())
                {
                    await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                             new DiscordInteractionResponseBuilder().WithContent("Соединение Lavalink не установлено"));
                }

                var node = lava.ConnectedNodes.Values.First();
                var voiceState = ctx.Member?.VoiceState;
                var channel = voiceState.Channel;

                if(channel.Type != ChannelType.Voice)
                {
                    await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                             new DiscordInteractionResponseBuilder().WithContent("Недопустимый голосовой канал"));
                }

                var conn = node.GetGuildConnection(channel.Guild);
                if(conn == null)
                {
                    await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                            new DiscordInteractionResponseBuilder().WithContent("Бот не находится в голосовом канале"));
                }

                await conn.DisconnectAsync();
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                            new DiscordInteractionResponseBuilder().WithContent($"Бот покинул {channel.Name}"));
            }

            [SlashCommand("pause", "Приостановить проигрыватель")]
            public async Task Pause(InteractionContext ctx)
            {
                if(ctx.Member.VoiceState == null || ctx.Member.VoiceState.Channel == null)
                {
                    await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                            new DiscordInteractionResponseBuilder().WithContent("Вы не на голосовом канале"));
                }

                var lava = ctx.Client.GetLavalink();
                var node = lava.ConnectedNodes.Values.First();
                var conn = node.GetGuildConnection(ctx.Member.VoiceState.Guild);

                if(conn == null)
                {
                    await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                            new DiscordInteractionResponseBuilder().WithContent("Бот не находится в голосовом канале"));
                }
                await conn.PauseAsync();
            }

            [SlashCommand("queue", "Очередь композиций")]
            public async Task ViewQueue(InteractionContext ctx)
            {
                string answer = "На очереди \n\n"; 
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

            public async Task<string> GetVideoTitle(InteractionContext ctx, YoutubeClient youtubeClient, string link)
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
                        return $"**Трек** __{author} - {title}__. \n**Длина**: __{totalMinutes}:{remainingSeconds:D2}__ \nзапустил";
                    }

                    return $"**Трек** __{author} - {title}__ добавлен в очередь";
                }
                catch (Exception)
                {
                    return null;
                }
            }

            public async Task LavaLinkPlaySound(InteractionContext ctx)
            {
                var lava = ctx.Client.GetLavalink();
                var nodes = lava.ConnectedNodes;
                var node = nodes.Values.First();
                var conn = node.GetGuildConnection(ctx.Member.VoiceState.Guild);

                conn.PlaybackFinished += async (sender, args) =>
                {
                    // Песня завершилась, переключаемся на следующий трек, если он есть в очереди
                    if (PlayList.Count > 0)
                    {
                        var nextTrack = PlayList.Dequeue();
                        var loadResult = await node.Rest.GetTracksAsync(nextTrack.Value);

                        if (loadResult.Tracks.Any())
                        {
                            var track = loadResult.Tracks.First();
                            await conn.PlayAsync(track);
                        }
                    }
                };

                // Воспроизводим первый трек
                if (PlayList.Count > 0)
                {
                    var nextTrack = PlayList.Dequeue();
                    var loadResult = await node.Rest.GetTracksAsync(nextTrack.Value);

                    if (loadResult.Tracks.Any())
                    {
                        var track = loadResult.Tracks.First();
                        await conn.PlayAsync(track);
                    }
                }
            }
        }
    }
}


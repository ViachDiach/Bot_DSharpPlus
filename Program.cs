using DSharpPlus;
using DSharpPlus.SlashCommands;
using static DSharpAPP.commands.Program;
using DSharpAPP.db;
using Newtonsoft.Json; 
using DSharpPlus.Entities;
using DSharpPlus.Lavalink;
using DSharpPlus.Net;
using DSharpPlus.EventArgs;

namespace MyFirstBot
{
    partial class Program
    {
        public class Config
        {
            public string Token { get; set; }
        }
        
        static async Task Main(string[] args)
        {
            var configPath = "Config.json";
            var configJson = File.ReadAllText(configPath);
            var config = JsonConvert.DeserializeObject<Config>(configJson);

            var discord = new DiscordClient(new DiscordConfiguration()
            {
                Token = config.Token, // Используем токен из конфигурации
                TokenType = TokenType.Bot,
                Intents = DiscordIntents.AllUnprivileged | DiscordIntents.MessageContents | DiscordIntents.GuildVoiceStates,
                
                
            });

            discord.GuildDownloadCompleted  += OnGuildDownloadCompleted;
            discord.ComponentInteractionCreated += ButtonPressResponce;
            discord.VoiceStateUpdated += Voice_State_Updated; 


            var endpoint = new ConnectionEndpoint
            {
                // Hostname = "90.188.250.219", // From your server configuration.
                Hostname = "127.0.0.1", // From your server configuration.
                Port = 80 // From your server configuration
            };


            // Password = "secret_password", // From your server configuration.
            var lavalinkConfig = new LavalinkConfiguration
            {
                // Password = "secret_password", // From your server configuration.
                Password = "youshallnotpass", // From your server configuration.
                RestEndpoint = endpoint,
                SocketEndpoint = endpoint
            };

            var lavalink = discord.UseLavalink();
            SQL.init(); // Инициализация подключения к базе данных

            var slashCommands = discord.UseSlashCommands(new SlashCommandsConfiguration
            {
                Services = ConfigureServices()
            });

            slashCommands.RegisterCommands<MoneyCommands>();
            slashCommands.RegisterCommands<ZamatCommands>();
            slashCommands.RegisterCommands<WeatherCommands>();
            slashCommands.RegisterCommands<ListCommands>();
            slashCommands.RegisterCommands<PresentsCommands>();
            slashCommands.RegisterCommands<StuffyCommands>();
            slashCommands.RegisterCommands<SwapCommands>();
            slashCommands.RegisterCommands<RadioCommands>();
            slashCommands.RegisterCommands<MusicCommands>();
           
            await discord.ConnectAsync();
            await lavalink.ConnectAsync(lavalinkConfig); // Make sure this is after Discord.ConnectAsync().

            await DSharpAPP.commands.CheckConnection.SendReport(discord);
            // await DSharpAPP.commands.CheckConnection.CreateAnswer(discord);

            await Task.Delay(-1);
        }

        private static async Task Voice_State_Updated(DiscordClient sender, VoiceStateUpdateEventArgs args)
        {
            var guild = args.Guild;
            if (guild == null)
                return;

            var userId = args.User.Id;
            var disconnectTime = DateTime.Now;

            var member = await guild.GetMemberAsync(userId);
            if (member == null)
                return;

            var voiceState = member.VoiceState;
            if (voiceState == null)
                return;

            if (voiceState.Channel != null)
            {
                Console.WriteLine($"{member.DisplayName} AND {member.Id} has joined voice channel {voiceState.Channel.Name}");
            }
            else
            {
                Console.WriteLine($"{member.DisplayName} AND  has left voice channel at");
            }
        }

        private static Task OnGuildDownloadCompleted(DiscordClient sender, GuildDownloadCompletedEventArgs args)
        {
            return Task.CompletedTask;
        }

        private static Task ButtonPressResponce(DiscordClient sender, DSharpPlus.EventArgs.ComponentInteractionCreateEventArgs e)
        {
            if (e.Interaction.Data.CustomId == "1") 
            {
                Console.WriteLine("You selected Option1");
            }
            
            return Task.CompletedTask;
        }

        private static IServiceProvider ConfigureServices()
        {
            return null;
        }
    }
}
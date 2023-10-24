using DSharpPlus;
using DSharpPlus.SlashCommands;
using static DSharpAPP.commands.Program;
using DSharpAPP.db;
using Newtonsoft.Json; 
using DSharpPlus.Lavalink;
using DSharpPlus.Net;

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
                Intents = DiscordIntents.AllUnprivileged | DiscordIntents.MessageContents
            });

            var endpoint = new ConnectionEndpoint
            {
                Hostname = "127.0.0.1", // From your server configuration.
                Port = 2333 // From your server configuration
            };

            var lavalinkConfig = new LavalinkConfiguration
            {
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
            slashCommands.RegisterCommands<MusicCommands>();
           
            await discord.ConnectAsync();
            await lavalink.ConnectAsync(lavalinkConfig); // Make sure this is after Discord.ConnectAsync().

            await Task.Delay(-1);
        }

        private static IServiceProvider ConfigureServices()
        {
            return null;
        }
    }
}
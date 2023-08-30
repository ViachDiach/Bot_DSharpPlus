using DSharpPlus;
using DSharpPlus.SlashCommands;
using static DSharpAPP.commands.Program;
using DSharpAPP.db;
using Newtonsoft.Json; 

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

            SQL.init(); // Инициализация подключения к базе данных

            var slashCommands = discord.UseSlashCommands(new SlashCommandsConfiguration
            {
                Services = ConfigureServices()
            });

            slashCommands.RegisterCommands<MoneyCommands>();
            slashCommands.RegisterCommands<ZamatCommands>();
            slashCommands.RegisterCommands<WeatherCommands>();
            slashCommands.RegisterCommands<ListCommands>();
            // slashCommands.RegisterCommands<PresentsCommands>();
            slashCommands.RegisterCommands<StuffyCommands>();
            slashCommands.RegisterCommands<SwapCommands>();
            
            // discord.GuildAvailable  += async (client, args) =>
            // {
            //     foreach (var guild  in client.Guilds.Values)
            //     {
            //         var textChannels = guild.Channels.Values.Where(channel => channel.Type == ChannelType.Text);

            //         foreach (var channel in textChannels)
            //         {
            //             await  channel.SendMessageAsync("Доброго времени суток");
            //         }
            //     }
            // };

            Console.WriteLine("Hello world");

            await discord.ConnectAsync();
            await Task.Delay(-1);
        }

        private static IServiceProvider ConfigureServices()
        {
            return null;
        }
    }
}
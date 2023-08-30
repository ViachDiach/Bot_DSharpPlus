using DSharpPlus;
using DSharpPlus.SlashCommands;
using DSharpPlus.Entities;
using Newtonsoft.Json;
using System.Net;

namespace DSharpAPP.commands
{
    public partial class Program
    {
        public class WeatherCommands : ApplicationCommandModule
        {   
            public class Config
            {
                public string ApiKey { get; set; }
            }
            private async Task<DiscordEmbedBuilder> WeatherParser(InteractionContext ctx, string town)
            {
                try
                {
                    var configPath = "Config.json";
                    var configJson = File.ReadAllText(configPath);
                    var config = JsonConvert.DeserializeObject<Config>(configJson);

                    var WebClient = new WebClient();
                    var MyApiKey = config.ApiKey; // Используем API-ключ из конфигурации
                    var json = WebClient.DownloadString($"https://api.openweathermap.org/data/2.5/weather?q={town}&appid={MyApiKey}&units=metric&lang=ru");
                    dynamic data = JsonConvert.DeserializeObject(json);

                    var cityName = data.name;
                    int minDegreesStr = data.main.temp_min;
                    int maxDegreesStr = data.main.temp_max;
                    int nowDegreesStr = data.main.feels_like;
                    var cloudStr = data.weather[0].description;
                    var humidity = data.main.humidity;
                    var windSpeed = data.wind.speed;
                    var icon = data.weather[0].icon;

                    string iconUrl = $"https://openweathermap.org/img/wn/{icon}@2x.png";

                    var embed = new DiscordEmbedBuilder()
                        .WithColor(DiscordColor.Blue)
                        .AddField("Минимально:", $"{minDegreesStr}°")
                        .AddField("Максимально:", $"{maxDegreesStr}°")
                        .AddField("Сейчас ощущается как:", $"{nowDegreesStr}°")
                        .AddField("Осадки:", $"{cloudStr}")
                        .AddField("Влажность:", $"{humidity}%")
                        .AddField("Скорость ветра:", $"{windSpeed} м/c")
                        .WithImageUrl(iconUrl);

                    return embed;
                }
                catch
                {
                    var errorEmbed = new DiscordEmbedBuilder()
                        .WithTitle($"Упс, что-то пошло не так")
                        .WithColor(DiscordColor.Red)
                        .AddField("Не удалось найти такой город.", "Попробуйте еще раз");

                    return errorEmbed;
                }
            }

            [SlashCommand("weather", "Погода в любом городе")]
            public async Task WeatherCommand(InteractionContext ctx, [Option("town", "town to search")] string townOption)
            {  
                if (townOption != null)
                {
                    var embed = await WeatherParser(ctx, townOption);

                    await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                        new DiscordInteractionResponseBuilder()
                            .WithContent($"Погода в городе {townOption} сегодня:")
                            .AddEmbed(embed)
                    );
                }
                else
                {
                    // Обработка случая, когда опция "town" отсутствует или команда задана некорректно
                    await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                        new DiscordInteractionResponseBuilder()
                            .WithContent("Не удалось получить город для погоды.")
                    );
                }

            }
        }
    }
}

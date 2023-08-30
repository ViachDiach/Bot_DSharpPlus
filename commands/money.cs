using DSharpPlus;
using DSharpPlus.SlashCommands;
using DSharpPlus.Entities;
using System.Net;
using Newtonsoft.Json;

namespace DSharpAPP.commands
{
    public partial class Program
    {
        public class MoneyCommands : ApplicationCommandModule
        {
            private void MoneyParser(out string day, out decimal eur, out decimal usd, out decimal cny, out decimal uah, out decimal kzt)
            {
                var webClient = new WebClient();
                var json = webClient.DownloadString("https://www.cbr-xml-daily.ru/latest.js");
                dynamic data = JsonConvert.DeserializeObject(json);

                eur = data.rates.EUR;
                usd = data.rates.USD;
                cny = data.rates.CNY;
                uah = data.rates.UAH;
                kzt = data.rates.KZT;
                day = data.date;
            }

            [SlashCommand("money", "курс ценных бумаг")]
            public async Task MoneyCommand(InteractionContext ctx)
            {
                string day;
                decimal eur, usd, cny, uah, kzt;

                MoneyParser(out day, out eur, out usd, out cny, out uah, out kzt); // Вызов MoneyParser для инициализации переменных

                var embed = new DiscordEmbedBuilder()
                    .WithTitle($"💰 Kypc валют на {day}")
                    .WithColor(DiscordColor.Gold)
                    .AddField("1 Евро €", $"{(1 / eur):F2} ₽ ")
                    .AddField("1 Доллар $", $"{(1 / usd):F2} ₽ ")
                    .AddField("1 Юань ¥", $"{(1 / cny):F2} ₽")
                    .AddField("1 Гривна ₴", $"{(1 / uah):F2} ₽ ")
                    .AddField("1 Тенге ₸", $"{(1 / kzt):F2} ₽ ")
                    .Build();

                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, 
                    new DiscordInteractionResponseBuilder()
                        .AddEmbed(embed));
            }
        }
    }
}

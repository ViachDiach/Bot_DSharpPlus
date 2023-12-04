using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.SlashCommands;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpAPP.commands;
using DSharpAPP.db;
using System.Diagnostics.CodeAnalysis;
using System.Formats.Asn1;
using System.Data.Entity.Validation;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.EventArgs;
using System.Text;
using System.Drawing;
using DSharpPlus.Interactivity.Enums;

namespace DSharpAPP.commands
{
    public class CheckConnection
    {
        public static async Task SendReport(DiscordClient discord)
        {
            var guildIdToCheck = 803324290308112415UL; 

            try
            {
            var guild = await discord.GetGuildAsync(guildIdToCheck);

            if (guild != null)
            {
                await Task.Delay(3000); // Добавим задержку перед циклом
  
                var members = await guild.GetAllMembersAsync().ToListAsync(); // Получим всех участников гильдии

                foreach (var member in members)
                {
                    SQL.AddConnectInfo(member.Username, guildIdToCheck.ToString());
                }
            }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Произошла ошибка: {ex.Message}");
            }
        }

        // public static async Task CreateAnswer(DiscordClient discord)
        // {
        //     // Создаем интерактивный ответ (таблица из Имя_Пользователя, Дата_Посл_Подключ + кнопка для тега)

        //     var guildIdToCheck = 884380677141831730UL; 
        //     ulong channelIdToSend = 980492159050453052UL;


        //     ulong test = 980492159050453052UL;

        //     List<string[]> connectInfo = SQL.GetConnectInfo(guildIdToCheck.ToString());

        //     var guild = await discord.GetGuildAsync(channelIdToSend); // Получаем объект гильдии по ID
        //     var channel = guild.GetChannelsAsync();  // Получаем объект канала по ID

        //     var button = new DiscordButtonComponent(ButtonStyle.Secondary, "tag_button", "🔔 Выбери кого тегнуть 🔔");

        //     DiscordEmbedBuilder embed = new DiscordEmbedBuilder();
        //     embed.Title = "";
        //     embed.Color = new DiscordColor(255, 215, 0);

        //     embed.AddField("Имя", $"`UserName`", true);
        //     embed.AddField("Когда был", "`дд:мм:гг`", true);
        //     embed.AddField("Длительность", "'мм:cc'", true);

        //     for (int i = 0; i < 7; i++)
        //     {
        //         embed.AddField("-----------", connectInfo[i][0], true);
        //         embed.AddField("-----------", connectInfo[i][1], true);
        //         embed.AddField("-----------", "654:43", true);
                
        //     }

        //     embed.Footer = new DiscordEmbedBuilder.EmbedFooter();
        //     Console.WriteLine("ШЛЕМ инфу");



        //     var messageBuilder = new DiscordMessageBuilder()
        //         .WithEmbed(embed)
        //         .AddComponents(button);

        //     if(guild != null)
        //     {   
        //         var defaultTextChannel = guild.Channels.Values.FirstOrDefault(channel => channel.Type == ChannelType.Text); // находим основной текстовый канал
        //         await defaultTextChannel.SendMessageAsync(messageBuilder);
        //     }
        // }


        // public static async Task CreateAnswer(DiscordClient discord)
        // {
        //     var guildIdToCheck = 884380677141831730UL;
        //     ulong channelIdToSend = 980492159050453052UL;
            

        //     List<string[]> connectInfo = SQL.GetConnectInfo(guildIdToCheck.ToString());

            
        //     var guild = await discord.GetGuildAsync(channelIdToSend); // Получаем объект гильдии по ID
        //     var channel = guild.GetChannelsAsync();  // Получаем объект канала по ID
        //     var defaultTextChannel = guild.Channels.Values.FirstOrDefault(channel => channel.Type == ChannelType.Text); // находим основной текстовый канал



        //     // Создаем список DiscordEmbed - это ваш контент для пейджей
        //     List<string> pages = new List<string>();

        //     // Генерируем эмбеды для каждой страницы
        //     int pageSize = connectInfo.Count; // Количество записей на странице
        //     for (int i = 0; i < connectInfo.Count; i += pageSize)
        //     {
        //         string pageContent = $"Page {i / pageSize + 1}\n\n"; // Заголовок страницы

        //         for (int j = i; j < Math.Min(i + pageSize, connectInfo.Count); j++)
        //         {
        //             pageContent += $"Имя: {connectInfo[j][0]}\nКогда был: {connectInfo[j][1]}\nДлительность: 654:43\n\n";
        //             // Добавляем информацию на страницу
        //         }

        //         pages.Add(pageContent);
        //     }

        //     Console.WriteLine("Готовим к отправке ");

        //     var paginator = new Peginator(discord, defaultTextChannel, pages);

        //     // Отправляем первую страницу
        //     await paginator.SendCurrentPageAsync();

        // }

        

       public static async Task CreateAnswer(DiscordClient discord)
        {
            var guildIdToCheck = 884380677141831730UL;
            ulong channelIdToSend = 980492159050453052UL;

            List<string[]> connectInfo = SQL.GetConnectInfo(guildIdToCheck.ToString());

            var guild = await discord.GetGuildAsync(channelIdToSend);
            var defaultTextChannel = guild.Channels.Values.FirstOrDefault(channel => channel.Type == ChannelType.Text);

            int pageSize = 2; // Количество записей на странице
            var pages = SplitArrayIntoPages(connectInfo, pageSize);

            // Отправляем каждую страницу поочередно
            await SendPagesAsync(discord, defaultTextChannel, pages);
        }

        private static List<string[][]> SplitArrayIntoPages(List<string[]> records, int pageSize)
        {
            List<string[][]> pages = new List<string[][]>();

            for (int i = 0; i < records.Count; i += pageSize)
            {
                var page = records.Skip(i).Take(pageSize).ToArray();
                pages.Add(page);
            }

            return pages;
        }

        private static async Task SendPagesAsync(DiscordClient discord, DiscordChannel channel, List<string[][]> pages)
        {
            var paginator = new Peginator(discord, channel, pages.Select(page => FormatPage(page)).ToList());

            // Отправляем первую страницу
            await paginator.SendCurrentPageAsync();
        }

        private static string FormatPage(string[][] page)
        {
            return string.Join(Environment.NewLine, page.Select(row => string.Join(", ", row)));
        }



    }
}
    
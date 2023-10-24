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

namespace DSharpAPP.commands
{
    public partial class Program
    {
        private static async Task<string> BuyNewRoleAsync(InteractionContext ctx, string roleName)
        {
            string answer = "";
            string author = ctx.User.Username;

            DiscordColor[] colorsList = new DiscordColor[]
            {
                DiscordColor.Green,
                DiscordColor.Red,
                DiscordColor.Blue,
                DiscordColor.DarkRed,
                DiscordColor.Magenta,
                DiscordColor.Orange,
                DiscordColor.Purple,
                DiscordColor.Teal,
            };

            DiscordGuild guild = ctx.Guild;
            var role = guild.Roles.FirstOrDefault(x => x.Value.Name == roleName).Value;

            if(role != null)
            {
                answer = "Роль с таким именем уже существует";
            }
            else
            {
                string[] balanse = SQL.CheckBalanse(author);
                string[] priceList = SQL.PriceMap("new_role");
                double price = -(double.Parse(priceList[1]));

                if(int.Parse(balanse[0])>4)
                {
                    Random random = new Random();
                    DiscordColor newColor = colorsList[random.Next(0, colorsList.Length)];
                    
                    var createdRole = await guild.CreateRoleAsync(roleName, mentionable: true, color: newColor); 
                    var socketRole = guild.GetRole(createdRole.Id);
                    
                    if(socketRole != null)
                    {
                        SQL.Updatebalanse(author, price);
                        answer = "Поздравляю, ты создал новую роль";
                    }
                }
                else
                    answer = "Увы, деняг недостаточно. Нужно еще подкопить";
            }

            return answer; 
        }

        private static async Task<string> BuyOldRole(InteractionContext ctx, string whom)
        {
            string author = ctx.User.Username;

            string[] balanse = SQL.CheckBalanse(author);
            string[] priceList = SQL.PriceMap("buy_role");
            double price = -(double.Parse(priceList[1]));
            string chatName = ctx.Guild.Name;

            Console.WriteLine("Начинаем выполнять");

            if (int.Parse(balanse[0]) > 3)
            {
                SQL.Updatebalanse(author, price);

                // Получение списка ролей на сервере
                var roles = ctx.Guild.Roles.Values.Where(role => role.Name != "@everyone").ToList();

                Console.WriteLine("Нашли список ролей");

                // Создание опций для выпадающего меню
                var options = roles.Select(role => new DiscordSelectComponentOption(role.Name, role.Name)); // CustomId здесь задаем имя роли

                Console.WriteLine("Создали опции");

                // Создание выпадающего меню
                var selectMenu = new DiscordSelectComponent("role-select", "Выберите роль", options); // Используем "role-select" как CustomId

                Console.WriteLine("Меню создали и генерируем сообщение");

                // Создаем сообщение
                var message = new DiscordMessageBuilder()
                    .WithContent($"Доступные роли на сервере **{chatName}**:")
                    .AddComponents(selectMenu);

                Console.WriteLine("Cebe");
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                    .WithContent($"Доступные роли на сервере **{chatName}**:")
                    .AddComponents(selectMenu));

                Console.WriteLine("Сообщение отправлено");

                return "Список ролей был отправлен в личное сообщение.";
            }
            else
                return "Увы, денег недостаточно. Нужно еще подкопить";
        }

        // Обработчик событий для выбора роли
        // private async Task HandleInteraction(SocketInteraction interaction)
        // {
        //     if (interaction is SocketMessageComponent messageComponent)
        //     {
        //         if (messageComponent.Data is ComponentInteractionData componentData)
        //         {
        //             if (componentData.CustomId == "role-select")
        //             {
        //                 string selectedRoleName = componentData.Values.FirstOrDefault();
        //                 Console.WriteLine($"Пользователь выбрал роль: {selectedRoleName}");
        //                 // Добавьте здесь код для выполнения действий, связанных с выбранной ролью.
        //             }
        //         }
        //     }
        // }

    }
}
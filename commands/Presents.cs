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
using System.Data;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;



namespace DSharpAPP.commands
{
    public partial class Program
    {
        public class PresentsCommands : ApplicationCommandModule
        {
            [SlashCommand("present", "Автомат подарков")]
            public async Task PresentCommand(InteractionContext ctx, 
                [Option("Choice", "Выбери что хочешь")] 
                [Choice("Test", "option0")]
                [Choice("Обозвать кого то", "option1")]
                [Choice("Сделать подарок", "option2")]
                [Choice("Купить роль", "option3")]
                [Choice("Создай роль", "option4")]
                [Choice("Проверь свой баланс", "option5")] 
                string choice,
                [Option("field", "Дополнительное поле")] // Дополнительное поле для выбора пользователя
                string  field = "default")
                {
                    DiscordEmbedBuilder embed = new DiscordEmbedBuilder();

                    switch (choice)
                    {
                        case "option0":
                            var programInstance = new Program(); // Создаем экземпляр класса Program
                            programInstance.GenerateTextInput(ctx);
                            break;

                        case "option1":
                            if (field != "default") 
                                embed.WithTitle($"{ctx.User.Username} бычара!");
                            else
                                embed.WithTitle("Укажите пользователя для обзывалки.");
                            break;

                        case "option2":
                            if (field != "default")
                                embed.WithTitle($"Подарок для {ctx.User.Username}!");
                            else
                                embed.WithTitle("Укажите пользователя для подарка");
                            break;

                        case "option3":
                            if (field != "default")
                            {
                                string output = await BuyOldRole(ctx, field);
                                
                                embed.WithTitle($"Hy смотри");
                                embed.WithColor(DiscordColor.SapGreen);
                                embed.WithDescription($"{ctx.User.Mention}, {output}");
                            }
                            else
                                embed.WithTitle("He указано (Себе/Кому-то(UserName)) в __field__");
                            break;


                             // DONE

                        case "option4":
                            if (field != "default")
                            {
                                string output = await BuyNewRoleAsync(ctx, field);

                                embed.WithTitle($"Hy смотри");
                                embed.WithColor(DiscordColor.SapGreen);
                                embed.WithDescription($"{output}");
                            }
                            else
                                embed.WithTitle("He указано название роли в __field__");
                            break;

                        case "option5":  
                            string author = ctx.User.Username;
                            string[] authorInfo = SQL.CheckBalanse(author);
                            
                                embed.WithTitle($"Hy смотри");
                                embed.WithColor(DiscordColor.SapGreen);
                                embed.WithDescription($"{ctx.User.Mention} \n**Баланс**  __{authorInfo[0]} шекелей__ \n**Инвентарь** : __{authorInfo[1]}__");
                            break;

                        default:
                                embed.WithTitle($"Погодите-ка...");
                                embed.WithColor(DiscordColor.Red);
                                embed.WithDescription($"Э, {ctx.User.Mention}, кэфтемэ, что то не так");
                            break;
                    }
                    
                    await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, 
                        new DiscordInteractionResponseBuilder()
                            .AddEmbed(embed));
                }
        }
    }
}
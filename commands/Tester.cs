using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;
using DSharpAPP.commands;

namespace DSharpAPP.commands
{
    public partial class Program
    {
        // Создаем событие для обработки нажатия кнопки в модальном окне
        private async Task GenerateTextInput(InteractionContext ctx)
        {
            try
            {
                Console.WriteLine("Генерирую");

                var textInput = new TextInputComponent(
                    "Укажи здесь как ты хочешь называть",
                    "test_test", // CustomId должен совпадать с тем, что мы проверяем в событии
                    required: false,
                    style: TextInputStyle.Short,
                    max_length: 20
                );

                Console.WriteLine("Шлем");

                var responseBuilder = new DiscordInteractionResponseBuilder()
                    .WithCustomId("test_test")
                    .WithTitle("Обзывалка тестовая")
                    .AddComponents(textInput);

        



        

                await ctx.CreateResponseAsync(InteractionResponseType.Modal, responseBuilder);

                try
                {
                    Console.WriteLine("Начало выполнения кода");

                    var res = await ctx.Client.GetInteractivity().WaitForModalAsync("test_test", TimeSpan.FromSeconds(30));
                    Console.WriteLine("Получен результат: " + res);
                    
                    if (!res.TimedOut)
                        {
                            if (res.Result != null && res.Result.Values != null && res.Result.Values.ContainsKey("test_test"))
                                Console.WriteLine("Получен результат: " + res.Result.Values["test_test"]);
                            else
                                Console.WriteLine("res.Result не содержит ожидаемых данных");
                        }
                        else
                            Console.WriteLine("Ожидание истекло");


                    Console.WriteLine("Завершение выполнения кода");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Произошла ошибка: " + ex.Message);
                }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}


                        ////// https://github.com/JulianusIV/LathBot/blob/bb4c526c9e04898e5e636384a1b0e73f403a735e/WarnModule/WarnBuilder.cs#L257
                        ///

                        //// https://github.com/JulianusIV/LathBot/blob/bb4c526c9e04898e5e636384a1b0e73f403a735e/LathBotFront/Interactions/WarnInteractions.cs#L533
                        ////

                        //// https://github.com/Flexlug/Skeletron/blob/17afac0fe2b9554107eb4fa71e30d4da7f8c2a01/src/Skeletron/ContextMenuCommands/AdminCommands.cs#L46
                        ////






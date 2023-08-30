using DSharpPlus;
using DSharpPlus.SlashCommands;
using DSharpPlus.Entities;
using DSharpAPP.db;

namespace DSharpAPP.commands
{
    public partial class Program
    {
        public class ZamatCommands : ApplicationCommandModule
        {
            private static string ListWords(int review)
            {
                string who = "";
                Random random = new Random();
                List<string> lst;

                var low = new List<string> {
                    "лучик солнца", "милашка", "прелесть", "снежинка", "умничка", "пушок", "лапочка", "крошка", "ягодка",
                    "персик", "конфетка", "ангелочек", "светлячок", "птенчик", "мурлыка", "котенок", "киса", "умница"};

                var mid = new List<string> {
                    "хвойда", "шльондра", "курва", "курвенний", "шльодравий", "хвойдяний", "курвар", "шльондер", "хвойдник",
                    "курварство", "піхва", "потка", "піхвяний", "піхвистий", "спіхварити", "вигнанець", "грайня", "алкомэн",
                    "гейропеец", "довбограник", "награйка", "прутень", "прутня", "прутнелиз", "прутнявий", "three hundred bucks",
                    "cum", "dungeon master", "полоумный", "худоумный", "человек очень среднего ума", "пузатый", "тюрюхайло", "нечёса", "рохля", "чванливый", "пыня",  
                    "boss of this gym", "fucking slave", "suck some dick", "чешский разбойник",
                    "балабоk", "рохля", "пузырь", "любопытный", "вошь", "бісова ковінька", "булька з носа", "пришелепкуватий",
                    "шмаркач", "тюхтій", "нездара", "дурепа", "йолоп", "боров", "быдло", "пустобрех", "вшивота", "трутень",
                    "лепешка", "обалдуй", "погань", "профурсетка", "хабалка", "хмырь", "shit", "bastard", "cunt",
                    "motherfucker", "fucking ass", "slut", "dumbass", "бикукле", "бикуля", "рередикт" };

                var high = new List<string> {
                    "фуфло", "душный козел", "свиняче рило", "підорко", "обезьяна", "шушера", "sucker", "son of a bitch",
                    "желчный", "бобик", "loser", "конь педальный", "геморрой", "шелупонь", "пердун"};

                if (review >= 0 && review <= 30) lst = low; 
                else if (review >= 31 && review <= 70) lst = mid; 
                else lst = high; 

                return  who = lst[random.Next(0, lst.Count)];	
            }

            [SlashCommand("zamat", "Хто ты cьoгoднi")]
            public async Task ZamatCommand(InteractionContext ctx)
            {
                string authorName = ctx.User.Username;
                string authorId = ctx.User.Id.ToString();

                string[] dataAnswer = SQL.Checking(authorName);

                DiscordEmbedBuilder embed;

                if (dataAnswer[3].Length == 0)
                {
                    int answer = SQL.Check(authorName);

                    if (answer != 0)
                    {
                        string curse = ListWords(answer);
                        SQL.AddCurse(authorId, curse);

                        embed = new DiscordEmbedBuilder()
                            .WithTitle($"Я бы не обижался, но")
                            .WithColor(DiscordColor.DarkGreen)
                            .WithDescription($"Ты сегодня гордо называешься - ***{curse}***");
                    }
                    else
                    {
                        embed = new DiscordEmbedBuilder()
                            .WithColor(DiscordColor.Red)
                            .AddField($"Воу-воу. Еще рано", "Сперва проверься на /stuffy");
                    }
                }
                else
                {
                    embed = new DiscordEmbedBuilder()
                        .WithTitle($"Погодите-ка")
                        .WithColor(DiscordColor.Magenta)
                        .AddField($"Ты уже есть в списке.", "Смотри /list");
                }

                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, 
                        new DiscordInteractionResponseBuilder()
                            .AddEmbed(embed));
            }
        }
    }
}

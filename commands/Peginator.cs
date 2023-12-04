using System;
using System.Collections.Generic;
using System.Security.AccessControl;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

namespace DSharpAPP.commands
{
    public class Peginator
    {
        private readonly DiscordClient _client;
        private readonly DiscordChannel _channel;
        private readonly List<string> _pages;
        private int _currentPage;
        private DiscordMessage _currentPageMessage;

        public Peginator(DiscordClient client, DiscordChannel channel, List<string> pages)
        {
            _client = client;
            _channel = channel;
            _pages = pages;
            _currentPage = 0;
        }

        public async Task SendCurrentPageAsync()
        {
            if (_currentPage < 0 || _currentPage >= _pages.Count)
            {
                throw new IndexOutOfRangeException("Invalid page number");
            }

            _currentPageMessage = await _channel.SendMessageAsync(_pages[_currentPage]);

            await AddPageButtonsAsync();
        }

        public async Task AddPageButtonsAsync()
        {
            Console.WriteLine("AddPageButtonsAsync");

            var buttons = new List<DiscordComponent>();

            var previousButton = new DiscordButtonComponent(ButtonStyle.Primary, "previous", "Previous");
            buttons.Add(previousButton);

            var nextButton = new DiscordButtonComponent(ButtonStyle.Primary, "next", "Next");
            buttons.Add(nextButton);
            
            // Создаем новое сообщение с текущим вложением и новыми компонентами
            var newMessage = new DiscordMessageBuilder()
                .WithContent(_currentPageMessage.Content) // Сохраняем текущий контент сообщения
                .WithEmbed(_currentPageMessage.Embeds[0]) // Сохраняем текущее вложение
                .AddComponents(new DiscordActionRowComponent(buttons)); // Добавляем новые компоненты

            // Удаляем старое сообщение
            await _currentPageMessage.DeleteAsync();

            // Отправляем новое сообщение
            await _channel.SendMessageAsync(newMessage);
        }


        public async Task HandleButtonInteractionAsync(ComponentInteractionCreateEventArgs e)
        {
            Console.WriteLine("HandleButtonInteractionAsync");

            if(_currentPageMessage == null || e.Message.Id != _currentPageMessage.Id)
                return;

            if(e.Id == "previous")
                await  PreviousPageAsync();
            else if(e.Id == "next")
                await NextPageAsync();       
        }
        public async Task GoToPageAsync(int pageNumber)
        {
            if (pageNumber < 0 || pageNumber >= _pages.Count)
            {
                throw new IndexOutOfRangeException("Invalid page number");
            }

            // Переходим на указанную страницу и обновляем сообщение
            _currentPage = pageNumber;
            await SendCurrentPageAsync();
        }

        public async Task NextPageAsync()
        {
            if (_currentPage < _pages.Count - 1)
            {
                // Переходим на следующую страницу и обновляем сообщение
                _currentPage++;
                await SendCurrentPageAsync();
            }
        }

        public async Task PreviousPageAsync()
        {
            if (_currentPage > 0)
            {
                // Переходим на предыдущую страницу и обновляем сообщение
                _currentPage--;
                await SendCurrentPageAsync();
            }
        }
    }
}
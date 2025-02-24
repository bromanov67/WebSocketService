using WebSocketService.Infrastructure;
//using WebSocketService.Cache;
using WebSocketService.Models;

namespace WebSocketService.Services
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _repository;
        //private readonly ICacheService _cacheService;

        public MessageService(IMessageRepository repository /*,ICacheService cacheService*/)
        {
            _repository = repository;
            //_cacheService = cacheService;
        }

        public void SendMessage(MessageDto messageDto)
        {
            // Создание нового сообщения с текущей меткой времени
            var message = new Message
            {
                Text = messageDto.Text,
                CreadtedAt = DateTime.UtcNow,
                OrderNumber = messageDto.OrderNumber
            };

            // Сохранение сообщения в базу данных
            _repository.SaveMessage(message);

            // Кэширование сообщения в Redis
            //_cacheService.SetCachedMessageAsync(message.Id, message.Text).Wait();
        }

        public List<Message> GetRecentMessages()
        {
            var fromTime = DateTime.UtcNow.AddMinutes(-10);
            var messages = _repository.GetRecentMessages(fromTime);

            var cachedMessages = new List<Message>();
            foreach (var message in messages)
            {
                //var cachedMessage = _cacheService.GetCachedMessageAsync(message.Id).Result;
                //if (!string.IsNullOrEmpty(cachedMessage))
                //{
                //    cachedMessages.Add(new Message { Id = message.Id, Text = cachedMessage, CreadtedAt = message.CreatedAt, OrderNumber = message.OrderNumber });
                //}
                //else
                //{
                //    cachedMessages.Add(message);
                //}
                cachedMessages.Add(message);

            }
            return cachedMessages;
        }

        public Message GetRandomMessage()
        {
            return _repository.GetRandomMessage();
        }
    }
}

using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using WebSocketService.Cache;
using WebSocketService.Infrastructure;
using WebSocketService.Models;

namespace WebSocketService.Services
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _repository;
        private readonly ICacheService _cacheService;
        private readonly IHubContext<MessageHub> _hubContext;
        private readonly ILogger<MessageService> _logger;

        public MessageService(
            IMessageRepository repository,
            ICacheService cacheService,
            IHubContext<MessageHub> hubContext,
            ILogger<MessageService> logger)
        {
            _repository = repository;
            _cacheService = cacheService;
            _hubContext = hubContext;
            _logger = logger;
        }

        public async Task SendMessageAsync(MessageDto messageDto, CancellationToken cancellation)
        {
            var message = new Message
            {
                Id = Guid.NewGuid(),
                Text = messageDto.Text,
                CreatedAt = DateTime.UtcNow,
                OrderNumber = messageDto.OrderNumber
            };

            try
            {
                await _repository.SaveMessageAsync(message);
                await _cacheService.SetCacheAsync(message.Id, message);
                await _hubContext.Clients.All.SendAsync("ReceiveMessage", message);
                _logger.LogInformation("Message {MessageId} processed successfully", message.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing message {MessageId}", message.Id);
                throw;
            }
        }

        public async Task<List<Message>> GetRecentMessagesAsync(CancellationToken cancellation)
        {
            _logger.LogInformation("Getting recent messages");

            try
            {
                var fromTime = DateTime.UtcNow.AddMinutes(-10);
                var messages = await _repository.GetRecentMessagesAsync(fromTime);
                _logger.LogDebug("Found {Count} messages in database", messages.Count);

                var cachedMessages = new List<Message>();
                foreach (var message in messages)
                {
                    try
                    {
                        var cachedMessage = await _cacheService.GetCacheAsync<Message>(message.Id);
                        cachedMessages.Add(cachedMessage ?? message);
                    }
                    catch (JsonException ex)
                    {
                        _logger.LogWarning(ex, "Cache deserialization error for message {MessageId}", message.Id);
                        cachedMessages.Add(message);
                    }
                }
                return cachedMessages;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving messages");
                throw;
            }
        }
    }
}
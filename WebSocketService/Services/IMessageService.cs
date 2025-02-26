using WebSocketService.Models;

namespace WebSocketService.Services
{
    public interface IMessageService
    {
        Task SendMessageAsync(MessageDto message, CancellationToken cancellation);
        Task <List<Message>> GetRecentMessagesAsync(CancellationToken cancellation);
    }
}
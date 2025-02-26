using WebSocketService.Models;

namespace WebSocketService.Infrastructure
{
    public interface IMessageRepository
    {
        Task SaveMessageAsync(Message message);
        Task <List<Message>> GetRecentMessagesAsync(DateTime fromTime);
    }
}

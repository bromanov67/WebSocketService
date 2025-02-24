using WebSocketService.Models;

namespace WebSocketService.Infrastructure
{
    public interface IMessageRepository
    {
        void SaveMessage(Message message);
        List<Message> GetRecentMessages(DateTime fromTime);
        Message GetRandomMessage();
    }
}

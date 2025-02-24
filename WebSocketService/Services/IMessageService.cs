using WebSocketService.Models;

namespace WebSocketService.Services
{
    public interface IMessageService
    {
        void SendMessage(MessageDto message);
        List<Message> GetRecentMessages();
        Message GetRandomMessage();
    }
}
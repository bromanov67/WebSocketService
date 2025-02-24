using Microsoft.AspNetCore.SignalR;
using WebSocketService.Models;

namespace WebSocketService.Controllers
{
    public class MessageHub : Hub
    {
        public async Task SendMessage(Message message)
        {
            await Clients.All.SendAsync("ReceiveMessage", message);
        }
    }
}

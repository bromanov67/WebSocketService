using Microsoft.AspNetCore.Mvc;
using WebSocketService.Models;
using WebSocketService.Services;

namespace WebSocketService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MessagesController : ControllerBase
    {
        private readonly IMessageService _messageService;

        public MessagesController(IMessageService messageService)
        {
            _messageService = messageService;
        }

        [HttpPost("send")]
        public IActionResult SendMessage([FromBody] MessageDto message)
        {
            _messageService.SendMessage(message);
            return Ok();
        }

        [HttpGet("history")]
        public IActionResult GetRecentMessages()
        {
            var messages = _messageService.GetRecentMessages();
            return Ok(messages);
        }

        [HttpGet("randomMessage")]
        public IActionResult GetRandomMessage()
        {
            var message = _messageService.GetRandomMessage();
            return Ok(message);
        }
    }

}

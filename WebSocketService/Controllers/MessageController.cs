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
        private readonly ILogger<MessagesController> _logger;

        public MessagesController(IMessageService messageService, ILogger<MessagesController> logger)
        {
            _messageService = messageService;
            _logger = logger;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendMessage([FromBody] MessageDto message, CancellationToken cancellation)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid message data received.");
                return BadRequest(ModelState);
            }

            try
            {
                _logger.LogInformation("Sending message: {Message}", message);
                await _messageService.SendMessageAsync(message, cancellation);
                _logger.LogInformation("Message sent successfully.");
                return StatusCode(201);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending message.");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetRecentMessages(CancellationToken cancellation)
        {
            try
            {
                _logger.LogInformation("Fetching recent messages.");
                var messages = await _messageService.GetRecentMessagesAsync(cancellation);

                if (messages == null || !messages.Any())
                {
                    _logger.LogInformation("No messages found.");
                    return NoContent();
                }

                _logger.LogInformation("Recent messages fetched successfully.");
                return Ok(messages);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching recent messages.");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }
    }
}

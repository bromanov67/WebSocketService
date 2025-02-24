namespace WebSocketService.Models
{
    public class MessageDto
    {
        public string Text { get; set; } = string.Empty;
        public DateTime CreadtedAt { get; set; }
        public int OrderNumber { get; set; }
    }
}

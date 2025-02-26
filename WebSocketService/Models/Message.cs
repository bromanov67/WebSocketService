namespace WebSocketService.Models
{
    public class Message
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Text { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public int OrderNumber { get; set; }
    }
}

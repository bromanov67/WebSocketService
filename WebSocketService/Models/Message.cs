namespace WebSocketService.Models
{
    public class Message
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public DateTime CreadtedAt { get; set; }
        public int OrderNumber { get; set; }
    }
}

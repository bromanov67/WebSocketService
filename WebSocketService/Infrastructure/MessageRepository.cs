using Npgsql;
using WebSocketService.Models;

namespace WebSocketService.Infrastructure
{
    public class MessageRepository : IMessageRepository
    {
        private readonly string _connectionString;

        public MessageRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public void SaveMessage(Message message)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();

                var query = "INSERT INTO Messages (Text, CreatedAt, OrderNumber) VALUES (@Text, @CreatedAt, @OrderNumber)";
                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Text", message.Text);
                    command.Parameters.AddWithValue("@CreatedAt", message.CreadtedAt);
                    command.Parameters.AddWithValue("@OrderNumber", message.OrderNumber);
                    command.ExecuteNonQuery();
                }
            }
        }

        public List<Message> GetRecentMessages(DateTime fromTime)
        {
            var messages = new List<Message>();

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();

                var query = "SELECT Id, Text, CreatedAt, OrderNumber FROM Messages WHERE CreatedAt >= @FromTime ORDER BY CreatedAt DESC";
                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@FromTime", fromTime);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var message = new Message
                            {
                                Id = reader.GetInt32(0),
                                Text = reader.GetString(1),
                                CreadtedAt = reader.GetDateTime(2),
                                OrderNumber = reader.GetInt32(3)
                            };
                            messages.Add(message);
                        }
                    }
                }
            }

            return messages;
        }

        public Message GetRandomMessage()
        {
            Message randomMessage = null;

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();

                var query = "SELECT Id, Text, CreatedAt, OrderNumber FROM Messages ORDER BY RANDOM() LIMIT 1";
                using (var command = new NpgsqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            randomMessage = new Message
                            {
                                Id = reader.GetInt32(0),
                                Text = reader.GetString(1),
                                CreadtedAt = reader.GetDateTime(2),
                                OrderNumber = reader.GetInt32(3)
                            };
                        }
                    }
                }
            }

            return randomMessage;
        }
    }
}

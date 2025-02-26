using Microsoft.Extensions.Logging;
using Npgsql;
using WebSocketService.Models;

namespace WebSocketService.Infrastructure
{
    public class MessageRepository : IMessageRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<MessageRepository> _logger;

        public MessageRepository(IConfiguration configuration, ILogger<MessageRepository> logger)
        {
            _logger = logger;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task SaveMessageAsync(Message message)
        {
            _logger.LogInformation("Saving message {MessageId}", message.Id);

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                try
                {
                    await connection.OpenAsync();
                    var query = "INSERT INTO messages (id, text, createdat, ordernumber) VALUES (@Id, @Text, @CreatedAt, @OrderNumber)";

                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("Id", message.Id);
                        command.Parameters.AddWithValue("Text", message.Text);
                        command.Parameters.AddWithValue("CreatedAt", message.CreatedAt);
                        command.Parameters.AddWithValue("OrderNumber", message.OrderNumber);

                        await command.ExecuteNonQueryAsync();
                    }
                    _logger.LogDebug("Message {MessageId} saved successfully", message.Id);
                }
                catch (NpgsqlException ex)
                {
                    _logger.LogError(ex, "Database error saving message {MessageId}", message.Id);
                    throw;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "General error saving message {MessageId}", message.Id);
                    throw;
                }
            }
        }


        public async Task<List<Message>> GetRecentMessagesAsync(DateTime fromTime)
        {
            _logger.LogInformation("Retrieving messages since {FromTime}", fromTime);

            var messages = new List<Message>();

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                try
                {
                    await connection.OpenAsync();
                    _logger.LogDebug("Database connection opened successfully.");

                    var query = "SELECT Id, Text, CreatedAt, OrderNumber FROM messages WHERE CreatedAt >= @FromTime ORDER BY CreatedAt DESC";
                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.Add("@FromTime", NpgsqlTypes.NpgsqlDbType.TimestampTz).Value = fromTime;

                        _logger.LogDebug("Executing query: {Query}", query);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var message = new Message
                                {
                                    Id = reader.GetGuid(0),
                                    Text = reader.GetString(1),
                                    CreatedAt = reader.GetDateTime(2),
                                    OrderNumber = reader.GetInt32(3)
                                };
                                messages.Add(message);
                            }
                        }
                    }
                    _logger.LogDebug("Retrieved {Count} messages", messages.Count);
                }
                catch (NpgsqlException ex)
                {
                    _logger.LogError(ex, "Database error retrieving messages since {FromTime}", fromTime);
                    throw;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "General error retrieving messages since {FromTime}", fromTime);
                    throw;
                }
            }
            return messages;
        }
    }
}
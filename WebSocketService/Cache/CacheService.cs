using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.Text.Json;

namespace WebSocketService.Cache
{
    public class CacheService : ICacheService
    {
        private readonly IDatabase _cache;
        private readonly ILogger<CacheService> _logger;

        public CacheService(IConfiguration configuration, ILogger<CacheService> logger)
        {
            _logger = logger;
            var redisConnectionString = configuration.GetConnectionString("Redis");

            try
            {
                var redisConnection = ConnectionMultiplexer.Connect(redisConnectionString);
                _cache = redisConnection.GetDatabase();
                _logger.LogInformation("Redis connection established");
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Failed to connect to Redis");
                throw;
            }
        }

        public async Task SetCacheAsync<T>(Guid key, T value) where T : class
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = true,
                };

                var serializedValue = JsonSerializer.Serialize(value, options);
                await _cache.StringSetAsync(key.ToString(), serializedValue);
                _logger.LogDebug("Cached value for key {Key}", key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error caching value for key {Key}", key);
                throw;
            }
        }

        public async Task<T?> GetCacheAsync<T>(Guid key) where T : class
        {
            try
            {
                var value = await _cache.StringGetAsync(key.ToString());

                if (!value.HasValue)
                {
                    _logger.LogDebug("Cache miss for key {Key}", key);
                    return null;
                }

                _logger.LogDebug("Cache hit for key {Key}", key);
                return JsonSerializer.Deserialize<T>(value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving cached value for key {Key}", key);
                throw;
            }
        }
    }
}
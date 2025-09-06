using Microsoft.Extensions.Logging;
using PS.FreeBookHub_Lite.PaymentService.Application.Interfaces.Redis;
using StackExchange.Redis;

namespace PS.FreeBookHub_Lite.PaymentService.Infrastructure.Caching.Redis
{
    public class RedisEventDeduplicationService : IEventDeduplicationService
    {
        private readonly IDatabase _database;
        private readonly ILogger<RedisEventDeduplicationService> _logger;
        public RedisEventDeduplicationService(
            IConnectionMultiplexer redis,
            ILogger<RedisEventDeduplicationService> logger)
        {
            _database = redis.GetDatabase();
            _logger = logger;
        }

        public async Task<bool> IsDuplicateAsync(string key, TimeSpan ttl, CancellationToken ct)
        {
            try
            {
                return !await _database.StringSetAsync(key, "1", ttl, When.NotExists);
            }
            catch (RedisConnectionException ex)
            {
                _logger.LogError(ex, "Redis connection failed");
                throw new Exception("Redis unavailable", ex);
                //throw new ServiceUnavailableException("Redis unavailable", ex);
            }
        }
    }
}

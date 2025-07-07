using PS.FreeBookHub_Lite.PaymentService.Application.Interfaces.Redis;
using StackExchange.Redis;

namespace PS.FreeBookHub_Lite.PaymentService.Infrastructure.Caching.Redis
{
    internal class RedisEventDeduplicationService : IEventDeduplicationService
    {
        private readonly IDatabase _database;
        public RedisEventDeduplicationService(IConnectionMultiplexer redis)
        {
            _database = redis.GetDatabase();
        }

        public async Task<bool> IsDuplicateAsync(string key, TimeSpan ttl, CancellationToken ct)
        {
            return !await _database.StringSetAsync(key, "1", ttl, When.NotExists);
        }
    }
}

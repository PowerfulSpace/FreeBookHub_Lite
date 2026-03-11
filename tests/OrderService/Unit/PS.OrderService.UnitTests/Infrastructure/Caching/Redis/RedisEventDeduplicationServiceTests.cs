using Microsoft.Extensions.Logging;
using Moq;
using PS.OrderService.Infrastructure.Caching.Redis;
using StackExchange.Redis;

namespace PS.OrderService.UnitTests.Infrastructure.Caching.Redis
{
    public class RedisEventDeduplicationServiceTests
    {
        private readonly Mock<IConnectionMultiplexer> _redisMock = new();
        private readonly Mock<IDatabase> _databaseMock = new();
        private readonly Mock<ILogger<RedisEventDeduplicationService>> _loggerMock = new();

        private RedisEventDeduplicationService CreateService()
        {
            _redisMock
                .Setup(r => r.GetDatabase(It.IsAny<int>(), It.IsAny<object>()))
                .Returns(_databaseMock.Object);

            return new RedisEventDeduplicationService(
                _redisMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task IsDuplicateAsync_ShouldReturnFalse_WhenKeyIsNew()
        {
            var service = CreateService();

            _databaseMock
                .Setup(db => db.StringSetAsync(
                    It.IsAny<RedisKey>(),
                    It.IsAny<RedisValue>(),
                    It.IsAny<TimeSpan?>(),
                    When.NotExists,
                    CommandFlags.None))
                .ReturnsAsync(true);

            var result = await service.IsDuplicateAsync("event-key", TimeSpan.FromMinutes(5), default);

            Assert.False(result);
        }
    }
}

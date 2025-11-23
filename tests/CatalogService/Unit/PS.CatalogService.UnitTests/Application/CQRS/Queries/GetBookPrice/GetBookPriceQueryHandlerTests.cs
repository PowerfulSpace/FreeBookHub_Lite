using Microsoft.Extensions.Logging;
using Moq;
using PS.CatalogService.Application.CQRS.Queries.GetBookPrice;
using PS.CatalogService.Application.Interfaces;
using PS.CatalogService.Domain.Entities;

namespace PS.CatalogService.UnitTests.Application.CQRS.Queries.GetBookPrice
{
    public class GetBookPriceQueryHandlerTests
    {
        private readonly Mock<IBookRepository> _repositoryMock = new();
        private readonly Mock<ILogger<GetBookPriceQueryHandler>> _loggerMock = new();

        private GetBookPriceQueryHandler CreateHandler() =>
            new(_repositoryMock.Object, _loggerMock.Object);

        [Fact]
        public async Task Handle_BookExists_ShouldReturnPrice()
        {
            // Arrange
            var id = Guid.NewGuid();
            var book = new Book { Id = id, Price = 49.99m };

            _repositoryMock
                .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(book);

            var handler = CreateHandler();
            var query = new GetBookPriceQuery(id);

            // Act
            var result = await handler.Handle(query, default);

            // Assert
            Assert.Equal(49.99m, result);

            _repositoryMock.Verify(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);

            _loggerMock.VerifyLog(LogLevel.Information, Times.Exactly(2)); // Started + Success
        }
    }

    public static class LoggerMockExtensions
    {
        public static void VerifyLog<T>(this Mock<ILogger<T>> loggerMock, LogLevel level, Times times)
        {
            loggerMock.Verify(
                x => x.Log(
                    level,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception?>(),
                    (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                times);
        }
    }
}

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

            var id = Guid.NewGuid();
            var book = new Book { Id = id, Price = 49.99m };

            _repositoryMock
                .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(book);

            var handler = CreateHandler();
            var query = new GetBookPriceQuery(id);


            var result = await handler.Handle(query, default);


            Assert.Equal(49.99m, result);

            _repositoryMock.Verify(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);

            _loggerMock.VerifyLog(LogLevel.Information, Times.Exactly(2)); // Started + Success
        }
    }
}

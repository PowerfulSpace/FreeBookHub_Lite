using Microsoft.Extensions.Logging;
using Moq;
using PS.CatalogService.Application.CQRS.Queries.GetBookPrice;
using PS.CatalogService.Application.Interfaces;

namespace PS.CatalogService.UnitTests.Application.CQRS.Queries.GetBookPrice
{
    public class GetBookPriceQueryHandlerTests
    {
        private readonly Mock<IBookRepository> _repositoryMock = new();
        private readonly Mock<ILogger<GetBookPriceQueryHandler>> _loggerMock = new();

        private GetBookPriceQueryHandler CreateHandler() =>
            new(_repositoryMock.Object, _loggerMock.Object);
    }
}

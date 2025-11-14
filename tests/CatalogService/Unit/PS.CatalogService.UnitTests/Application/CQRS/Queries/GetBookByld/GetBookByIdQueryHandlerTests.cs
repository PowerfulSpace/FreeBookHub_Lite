using Microsoft.Extensions.Logging;
using Moq;
using PS.CatalogService.Application.CQRS.Queries.GetBookById;
using PS.CatalogService.Application.Interfaces;

namespace PS.CatalogService.UnitTests.Application.CQRS.Queries.GetBookByld
{
    public class GetBookByIdQueryHandlerTests
    {
        private readonly Mock<IBookRepository> _repositoryMock = new();
        private readonly Mock<ILogger<GetBookByIdQueryHandler>> _loggerMock = new();

        private GetBookByIdQueryHandler CreateHandler() =>
            new(_repositoryMock.Object, _loggerMock.Object);
    }
}

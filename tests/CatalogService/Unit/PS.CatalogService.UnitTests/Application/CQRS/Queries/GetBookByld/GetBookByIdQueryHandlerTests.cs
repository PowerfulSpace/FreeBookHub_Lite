using Microsoft.Extensions.Logging;
using Moq;
using PS.CatalogService.Application.CQRS.Queries.GetBookById;
using PS.CatalogService.Application.Interfaces;
using PS.CatalogService.Domain.Entities;

namespace PS.CatalogService.UnitTests.Application.CQRS.Queries.GetBookByld
{
    public class GetBookByIdQueryHandlerTests
    {
        private readonly Mock<IBookRepository> _repositoryMock = new();
        private readonly Mock<ILogger<GetBookByIdQueryHandler>> _loggerMock = new();

        private GetBookByIdQueryHandler CreateHandler() =>
            new(_repositoryMock.Object, _loggerMock.Object);


        [Fact]
        public async Task Handle_ShouldReturnMappedBook()
        {
            // Arrange
            var id = Guid.NewGuid();

            var book = new Book
            {
                Id = id,
                Title = "Test Title",
                Author = "Test Author",
                Description = "Desc",
                ISBN = "123",
                Price = 50,
                PublishedAt = DateTime.UtcNow,
                CoverImageUrl = "img"
            };

            _repositoryMock
                .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(book);

            var handler = CreateHandler();
            var query = new GetBookByIdQuery(id);

            // Act
            var result = await handler.Handle(query, default);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(book.Title, result.Title);
            Assert.Equal(book.ISBN, result.ISBN);

            _repositoryMock.Verify(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
            _loggerMock.VerifyLog(LogLevel.Information, Times.AtLeast(2));
        }
    }
}

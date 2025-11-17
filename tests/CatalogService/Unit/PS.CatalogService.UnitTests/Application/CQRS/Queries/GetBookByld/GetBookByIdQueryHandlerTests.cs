using Microsoft.Extensions.Logging;
using Moq;
using PS.CatalogService.Application.CQRS.Queries.GetBookById;
using PS.CatalogService.Application.Interfaces;
using PS.CatalogService.Domain.Entities;
using PS.CatalogService.Domain.Exceptions.Book;

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

        [Fact]
        public async Task Handle_BookNotFound_ShouldThrowBookNotFoundException()
        {
            // Arrange
            var id = Guid.NewGuid();

            _repositoryMock
                .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Book?)null);

            var handler = CreateHandler();
            var query = new GetBookByIdQuery(id);

            // Act & Assert
            await Assert.ThrowsAsync<BookNotFoundException>(() => handler.Handle(query, default));

            _repositoryMock.Verify(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
            _loggerMock.VerifyLog(LogLevel.Information, Times.Once); // only "Started" logged before exception
        }
    }

    // 🔧 Вспомогательное расширение для проверки логов
    internal static class LoggerMockExtensions
    {
        public static void VerifyLog<T>(this Mock<ILogger<T>> loggerMock, LogLevel level, Times times)
        {
            loggerMock.Verify(
                l => l.Log(
                    level,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception?>(),
                    (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                times);
        }
    }
}

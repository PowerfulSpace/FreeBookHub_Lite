using Microsoft.Extensions.Logging;
using Moq;
using PS.CatalogService.Application.CQRS.Commands.DeleteBook;
using PS.CatalogService.Application.Interfaces;
using PS.CatalogService.Domain.Entities;

namespace PS.CatalogService.UnitTests.Application.CQRS.Commands.DeleteBook
{
    public class DeleteBookCommandHandlerTests
    {
        private readonly Mock<IBookRepository> _bookRepoMock = new();
        private readonly Mock<ILogger<DeleteBookCommandHandler>> _loggerMock = new();

        private DeleteBookCommandHandler CreateHandler() =>
            new(_bookRepoMock.Object, _loggerMock.Object);

        [Fact]
        public async Task Handle_BookExists_ShouldDeleteAndReturnTrue()
        {
            // Arrange
            var bookId = Guid.NewGuid();
            var command = new DeleteBookCommand { Id = bookId };
            var book = new Book { Id = bookId, Title = "Sample Book" };

            _bookRepoMock.Setup(r => r.GetByIdAsync(bookId, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(book);

            _bookRepoMock.Setup(r => r.DeleteAsync(bookId, It.IsAny<CancellationToken>()))
                         .Returns(Task.CompletedTask);

            var handler = CreateHandler();

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            Assert.True(result);
            _bookRepoMock.Verify(r => r.DeleteAsync(bookId, It.IsAny<CancellationToken>()), Times.Once);
            _loggerMock.VerifyLog(LogLevel.Information, Times.AtLeast(2)); // Started + Success
        }
    }

    // 🔧 Вспомогательное расширение для проверки логов (удобно переиспользовать)
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

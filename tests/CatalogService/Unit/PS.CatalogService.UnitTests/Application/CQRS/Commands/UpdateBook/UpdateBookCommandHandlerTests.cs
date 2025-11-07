using Microsoft.Extensions.Logging;
using Moq;
using PS.CatalogService.Application.CQRS.Commands.UpdateBook;
using PS.CatalogService.Application.Interfaces;
using PS.CatalogService.Domain.Entities;
using PS.CatalogService.Domain.Exceptions.Book;
using PS.CatalogService.UnitTests.Application.CQRS.Commands.DeleteBook;

namespace PS.CatalogService.UnitTests.Application.CQRS.Commands.UpdateBook
{
    public class UpdateBookCommandHandlerTests
    {
        private readonly Mock<IBookRepository> _bookRepoMock = new();
        private readonly Mock<ILogger<UpdateBookCommandHandler>> _loggerMock = new();

        private UpdateBookCommandHandler CreateHandler() =>
            new(_bookRepoMock.Object, _loggerMock.Object);

        [Fact]
        public async Task Handle_BookExists_ShouldUpdateAndReturnTrue()
        {
            // Arrange
            var bookId = Guid.NewGuid();
            var existingBook = new Book
            {
                Id = bookId,
                Title = "Old Title",
                Author = "Old Author",
                Price = 5m
            };

            var command = new UpdateBookCommand
            {
                Id = bookId,
                Title = "New Title",
                Author = "New Author",
                Description = "Updated",
                ISBN = "12345",
                Price = 10m,
                PublishedAt = DateTime.UtcNow,
                CoverImageUrl = "http://example.com/cover.jpg"
            };

            _bookRepoMock.Setup(r => r.GetByIdAsync(bookId, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(existingBook);

            _bookRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Book>(), It.IsAny<CancellationToken>()))
                         .Returns(Task.CompletedTask);

            var handler = CreateHandler();

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            Assert.True(result);
            _bookRepoMock.Verify(r => r.UpdateAsync(It.Is<Book>(b =>
                b.Id == bookId &&
                b.Title == "New Title" &&
                b.Author == "New Author" &&
                b.Price == 10m
            ), It.IsAny<CancellationToken>()), Times.Once);

            _loggerMock.VerifyLog(LogLevel.Information, Times.AtLeast(2)); // Started + Success
        }

        [Fact]
        public async Task Handle_BookNotFound_ShouldThrowBookNotFoundException()
        {
            // Arrange
            var command = new UpdateBookCommand
            {
                Id = Guid.NewGuid(),
                Title = "Doesn't Matter"
            };

            _bookRepoMock.Setup(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
                         .ReturnsAsync((Book?)null);

            var handler = CreateHandler();

            // Act + Assert
            await Assert.ThrowsAsync<BookNotFoundException>(() => handler.Handle(command, default));

            _bookRepoMock.Verify(r => r.UpdateAsync(It.IsAny<Book>(), It.IsAny<CancellationToken>()), Times.Never);
        }


        [Fact]
        public async Task Handle_ShouldLog_StartedAndSuccessMessages()
        {
            // Arrange
            var bookId = Guid.NewGuid();
            var existingBook = new Book { Id = bookId, Title = "Old Title" };

            var command = new UpdateBookCommand
            {
                Id = bookId,
                Title = "Updated Title"
            };

            _bookRepoMock.Setup(r => r.GetByIdAsync(bookId, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(existingBook);

            var handler = CreateHandler();

            // Act
            await handler.Handle(command, default);

            // Assert
            _loggerMock.VerifyLog(LogLevel.Information, Times.AtLeast(2));
        }
    }
}


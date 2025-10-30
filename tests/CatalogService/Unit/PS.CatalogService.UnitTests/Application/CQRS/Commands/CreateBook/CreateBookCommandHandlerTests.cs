using Microsoft.Extensions.Logging;
using Moq;
using PS.CatalogService.Application.CQRS.Commands.CreateBook;
using PS.CatalogService.Application.Interfaces;
using PS.CatalogService.Domain.Entities;

namespace PS.CatalogService.UnitTests.Application.CQRS.Commands.CreateBook
{
    public class CreateBookCommandHandlerTests
    {
        private readonly Mock<IBookRepository> _bookRepoMock = new();
        private readonly Mock<ILogger<CreateBookCommandHandler>> _loggerMock = new();

        private CreateBookCommandHandler CreateHandler() =>
            new(_bookRepoMock.Object, _loggerMock.Object);

        [Fact]
        public async Task Handle_ShouldCreateBookSuccessfully()
        {
            // Arrange
            var command = new CreateBookCommand
            {
                Title = "Clean Architecture",
                Author = "Robert C. Martin",
                Description = "A book about software architecture best practices.",
                ISBN = "978-0134494166",
                Price = 45.99m,
                PublishedAt = new DateTime(2017, 9, 20),
                CoverImageUrl = "https://example.com/cover.jpg"
            };

            Book? addedBook = null;

            _bookRepoMock.Setup(r => r.AddAsync(It.IsAny<Book>(), It.IsAny<CancellationToken>()))
                         .Callback<Book, CancellationToken>((b, _) => addedBook = b)
                         .Returns(Task.CompletedTask);

            var handler = CreateHandler();

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(command.Title, result.Title);
            Assert.Equal(command.Author, result.Author);
            Assert.Equal(command.Description, result.Description);
            Assert.Equal(command.ISBN, result.ISBN);
            Assert.Equal(command.Price, result.Price);
            Assert.Equal(command.PublishedAt, result.PublishedAt);
            Assert.Equal(command.CoverImageUrl, result.CoverImageUrl);

            // Проверим что AddAsync вызван ровно 1 раз
            _bookRepoMock.Verify(r => r.AddAsync(It.IsAny<Book>(), It.IsAny<CancellationToken>()), Times.Once);

            // Проверим что книга действительно создана и Id проставлен
            Assert.NotNull(addedBook);
            Assert.NotEqual(Guid.Empty, addedBook!.Id);
        }
        [Fact]
        public async Task Handle_ShouldLogInformation_OnStartAndSuccess()
        {
            // Arrange
            var command = new CreateBookCommand
            {
                Title = "Domain-Driven Design",
                Author = "Eric Evans",
                Description = "Foundational DDD book.",
                ISBN = "978-0321125217",
                Price = 59.99m,
                PublishedAt = new DateTime(2003, 8, 30),
                CoverImageUrl = "https://example.com/ddd.jpg"
            };

            _bookRepoMock.Setup(r => r.AddAsync(It.IsAny<Book>(), It.IsAny<CancellationToken>()))
                         .Returns(Task.CompletedTask);

            var handler = CreateHandler();

            // Act
            await handler.Handle(command, default);

            // Assert: логирование вызвано хотя бы дважды (Start и Success)
            _loggerMock.Verify(
                l => l.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception?>(),
                    (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.AtLeast(2));
        }

    }
}

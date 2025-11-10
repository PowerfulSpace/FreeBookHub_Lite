using Microsoft.Extensions.Logging;
using Moq;
using PS.CatalogService.Application.CQRS.Queries.GetAllBooks;
using PS.CatalogService.Application.Interfaces;
using PS.CatalogService.Domain.Entities;

namespace PS.CatalogService.UnitTests.Application.CQRS.Queries.GetAllBooks
{
    public class GetAllBooksQueryHandlerTests
    {
        private readonly Mock<IBookRepository> _repositoryMock = new();
        private readonly Mock<ILogger<GetAllBooksQueryHandler>> _loggerMock = new();

        private GetAllBooksQueryHandler CreateHandler() =>
            new(_repositoryMock.Object, _loggerMock.Object);

        [Fact]
        public async Task Handle_ShouldReturnMappedBooks()
        {
            // Arrange
            var books = new List<Book>
            {
                new Book
                {
                    Id = Guid.NewGuid(),
                    Title = "Book 1",
                    Author = "Author 1",
                    Description = "Desc 1",
                    ISBN = "111",
                    Price = 10,
                    PublishedAt = DateTime.UtcNow,
                    CoverImageUrl = "url1"
                },
                new Book
                {
                    Id = Guid.NewGuid(),
                    Title = "Book 2",
                    Author = "Author 2",
                    Description = "Desc 2",
                    ISBN = "222",
                    Price = 20,
                    PublishedAt = DateTime.UtcNow,
                    CoverImageUrl = "url2"
                }
            };

            _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                           .ReturnsAsync(books);

            var handler = CreateHandler();
            var query = new GetAllBooksQuery();

            // Act
            var result = await handler.Handle(query, default);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(books.Count, result.Count());

            var resultList = result.ToList();
            Assert.Equal(books[0].Title, resultList[0].Title);
            Assert.Equal(books[1].Author, resultList[1].Author);

            _repositoryMock.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
            _loggerMock.VerifyLog(LogLevel.Information, Times.AtLeast(2));
        }
    }
}

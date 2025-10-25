using PS.CatalogService.Domain.Entities;

namespace PS.CatalogService.UnitTests.Domain
{
    public class BookTests
    {
        [Fact]
        public void CreateBook_ShouldInitializeWithValidId()
        {
            // Act
            var book = new Book();

            // Assert
            Assert.NotEqual(Guid.Empty, book.Id);
        }

        [Fact]
        public void Should_SetAndGet_AllPropertiesCorrectly()
        {
            // Arrange
            var id = Guid.NewGuid();
            var title = "The Pragmatic Programmer";
            var author = "Andrew Hunt";
            var description = "A must-read book for software developers.";
            var isbn = "978-0201616224";
            var price = 49.99m;
            var publishedAt = new DateTime(1999, 10, 30);
            var imageUrl = "https://example.com/cover.jpg";

            // Act
            var book = new Book
            {
                Id = id,
                Title = title,
                Author = author,
                Description = description,
                ISBN = isbn,
                Price = price,
                PublishedAt = publishedAt,
                CoverImageUrl = imageUrl
            };

            // Assert
            Assert.Equal(id, book.Id);
            Assert.Equal(title, book.Title);
            Assert.Equal(author, book.Author);
            Assert.Equal(description, book.Description);
            Assert.Equal(isbn, book.ISBN);
            Assert.Equal(price, book.Price);
            Assert.Equal(publishedAt, book.PublishedAt);
            Assert.Equal(imageUrl, book.CoverImageUrl);
        }

        [Fact]
        public void Should_AllowChangingPropertyValues()
        {
            var book = new Book
            {
                Title = "Old Title",
                Price = 10m
            };

            book.Title = "New Title";
            book.Price = 15m;

            Assert.Equal("New Title", book.Title);
            Assert.Equal(15m, book.Price);
        }
    }
}

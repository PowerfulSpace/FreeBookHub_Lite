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
    }
}

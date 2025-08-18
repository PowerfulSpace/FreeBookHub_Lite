using PS.FreeBookHub_Lite.CartService.Domain.Entities;

namespace CartService.UnitTests.Domain
{
    public class CartItemTests
    {
        [Fact]
        public void CreateCartItem_ShouldInitializeCorrectly()
        {
            var bookId = Guid.NewGuid();
            var quantity = 2;
            var unitPrice = 10m;

            var item = new CartItem(bookId, quantity, unitPrice);

            Assert.Equal(bookId, item.BookId);
            Assert.Equal(quantity, item.Quantity);
            Assert.Equal(unitPrice, item.UnitPrice);
            Assert.Equal(quantity * unitPrice, item.TotalPrice);
        }
    }
}

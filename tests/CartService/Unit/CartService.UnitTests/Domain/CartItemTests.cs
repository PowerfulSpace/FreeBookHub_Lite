using PS.FreeBookHub_Lite.CartService.Domain.Entities;
using PS.FreeBookHub_Lite.CartService.Domain.Exceptions.Cart;

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

        [Fact]
        public void UpdateQuantity_ShouldChangeQuantity_WhenValid()
        {
            var item = new CartItem(Guid.NewGuid(), 2, 10m);

            item.UpdateQuantity(5);

            Assert.Equal(5, item.Quantity);
            Assert.Equal(50m, item.TotalPrice);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-10)]
        public void UpdateQuantity_ShouldThrow_WhenQuantityIsZeroOrNegative(int invalidQuantity)
        {
            var item = new CartItem(Guid.NewGuid(), 2, 10m);

            var ex = Assert.Throws<InvalidCartItemQuantityException>(
                () => item.UpdateQuantity(invalidQuantity)
            );

            Assert.Equal(invalidQuantity, ex.Quantity);
        }

        [Fact]
        public void TotalPrice_ShouldReflectQuantityChanges()
        {
            var item = new CartItem(Guid.NewGuid(), 1, 15m);

            Assert.Equal(15m, item.TotalPrice);

            item.UpdateQuantity(3);

            Assert.Equal(45m, item.TotalPrice);
        }
    }
}

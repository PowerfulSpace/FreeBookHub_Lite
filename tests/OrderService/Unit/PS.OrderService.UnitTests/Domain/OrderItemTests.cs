using PS.OrderService.Domain.Entities;
using PS.OrderService.Domain.Exceptions.Order;
using StackExchange.Redis;

namespace PS.OrderService.UnitTests.Domain
{
    public class OrderItemTests
    {
        [Fact]
        public void CreateOrderItem_ShouldInitializeCorrectly()
        {
            // Arrange
            var bookId = Guid.NewGuid();
            var price = 25m;
            var quantity = 2;

            // Act
            var item = new OrderItem(bookId, price, quantity);

            // Assert
            Assert.Equal(bookId, item.BookId);
            Assert.Equal(price, item.UnitPrice);
            Assert.Equal(quantity, item.Quantity);
            Assert.Equal(50m, item.TotalPrice);
        }

        [Fact]
        public void TotalPrice_ShouldBeCalculatedCorrectly()
        {
            // Arrange
            var item = new OrderItem(Guid.NewGuid(), 15m, 3);

            // Act
            var total = item.TotalPrice;

            // Assert
            Assert.Equal(45m, total);
        }

        [Fact]
        public void UpdateQuantity_ShouldUpdate_WhenQuantityIsValid()
        {
            // Arrange
            var item = new OrderItem(Guid.NewGuid(), 10m, 1);

            // Act
            item.UpdateQuantity(5);

            // Assert
            Assert.Equal(5, item.Quantity);
            Assert.Equal(50m, item.TotalPrice);
        }


        [Fact]
        public void UpdateQuantity_WhenZeroOrNegative_ShouldThrow()
        {
            // Arrange
            var item = new OrderItem(Guid.NewGuid(), 10m, 1);

            // Act
            var exZero = Assert.Throws<InvalidOrderQuantityException>(() =>
                item.UpdateQuantity(0));

            var exNegative = Assert.Throws<InvalidOrderQuantityException>(() =>
                item.UpdateQuantity(-3));

            // Assert
            Assert.Equal(0, exZero.ProvidedQuantity);
            Assert.Equal(-3, exNegative.ProvidedQuantity);
        }
    }
}

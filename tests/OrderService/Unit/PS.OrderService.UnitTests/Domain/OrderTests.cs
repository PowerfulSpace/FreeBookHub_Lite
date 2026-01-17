using PS.OrderService.Domain.Entities;
using PS.OrderService.Domain.Enums;
using PS.OrderService.Domain.Exceptions.Order;

namespace PS.OrderService.UnitTests.Domain
{
    public class OrderTests
    {
        [Fact]
        public void CreateOrder_ShouldInitializeCorrectly()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var address = "Some address";

            // Act
            var order = new Order(userId, address);

            // Assert
            Assert.NotEqual(Guid.Empty, order.Id);
            Assert.Equal(userId, order.UserId);
            Assert.Equal(address, order.ShippingAddress);
            Assert.Equal(OrderStatus.New, order.Status);
            Assert.True(order.CreatedAt <= DateTime.UtcNow);
            Assert.Empty(order.Items);
        }

        [Fact]
        public void AddItem_ShouldAddNewItem()
        {
            // Arrange
            var order = new Order(Guid.NewGuid(), "address");
            var bookId = Guid.NewGuid();

            // Act
            order.AddItem(bookId, 10m, 2);

            // Assert
            Assert.Single(order.Items);
            var item = order.Items.First();
            Assert.Equal(bookId, item.BookId);
            Assert.Equal(2, item.Quantity);
            Assert.Equal(20m, item.TotalPrice);
        }

        [Fact]
        public void AddItem_WhenItemExists_ShouldIncreaseQuantity()
        {
            // Arrange
            var order = new Order(Guid.NewGuid(), "address");
            var bookId = Guid.NewGuid();

            // Act
            order.AddItem(bookId, 10m, 2);
            order.AddItem(bookId, 10m, 3);

            // Assert
            var item = order.Items.First();
            Assert.Equal(5, item.Quantity);
            Assert.Equal(50m, item.TotalPrice);
        }

        [Fact]
        public void AddItem_WithInvalidQuantity_ShouldThrow()
        {
            // Arrange
            var order = new Order(Guid.NewGuid(), "address");

            // Act
            var ex = Assert.Throws<InvalidOrderQuantityException>(() =>
                order.AddItem(Guid.NewGuid(), 10m, 0));

            // Assert
            Assert.Equal(0, ex.ProvidedQuantity);
        }


        [Fact]
        public void RemoveItem_ShouldRemoveExistingItem()
        {
            var order = new Order(Guid.NewGuid(), "address");
            var bookId = Guid.NewGuid();

            order.AddItem(bookId, 10m, 1);

            order.RemoveItem(bookId);

            Assert.Empty(order.Items);
        }
    }
}


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
            // Arrange
            var order = new Order(Guid.NewGuid(), "address");
            var bookId = Guid.NewGuid();

            order.AddItem(bookId, 10m, 1);

            // Act
            order.RemoveItem(bookId);

            // Assert
            Assert.Empty(order.Items);
        }

        [Fact]
        public void RemoveItem_WhenItemDoesNotExist_ShouldNotThrow()
        {
            // Arrange
            var order = new Order(Guid.NewGuid(), "address");

            // Act
            order.RemoveItem(Guid.NewGuid());

            // Assert
            Assert.Empty(order.Items);
        }

        [Fact]
        public void TotalPrice_ShouldReturnSumOfItems()
        {
            // Arrange
            var order = new Order(Guid.NewGuid(), "address");

            // Act
            order.AddItem(Guid.NewGuid(), 10m, 2); // 20
            order.AddItem(Guid.NewGuid(), 5m, 3);  // 15

            // Assert
            Assert.Equal(35m, order.TotalPrice);
        }

        [Fact]
        public void Cancel_WhenStatusIsNew_ShouldSetCancelled()
        {
            // Arrange
            var order = new Order(Guid.NewGuid(), "address");

            // Act
            order.Cancel();

            // Assert
            Assert.Equal(OrderStatus.Cancelled, order.Status);
        }

        [Theory]
        [InlineData(OrderStatus.Shipped)]
        [InlineData(OrderStatus.Delivered)]
        [InlineData(OrderStatus.Cancelled)]
        public void Cancel_WhenStatusIsInvalid_ShouldThrow(OrderStatus status)
        {
            // Arrange
            var order = new Order(Guid.NewGuid(), "address");

            typeof(Order)
                .GetProperty(nameof(Order.Status))!
                .SetValue(order, status);

            // Act
            var ex = Assert.Throws<CannotCancelOrderException>(() => order.Cancel());

            // Assert
            Assert.Equal(order.Id, ex.OrderId);
            Assert.Equal(status, ex.CurrentStatus);
        }

        [Fact]
        public void MarkAsPaid_WhenStatusIsNew_ShouldSetPaid()
        {
            // Arrange
            var order = new Order(Guid.NewGuid(), "address");

            // Act
            order.MarkAsPaid();

            // Assert
            Assert.Equal(OrderStatus.Paid, order.Status);
        }

        [Theory]
        [InlineData(OrderStatus.Paid)]
        [InlineData(OrderStatus.Shipped)]
        [InlineData(OrderStatus.Delivered)]
        [InlineData(OrderStatus.Cancelled)]
        public void MarkAsPaid_WhenStatusIsNotNew_ShouldThrow(OrderStatus status)
        {
            var order = new Order(Guid.NewGuid(), "address");

            typeof(Order)
                .GetProperty(nameof(Order.Status))!
                .SetValue(order, status);

            var ex = Assert.Throws<InvalidOrderPaymentStateException>(() =>
                order.MarkAsPaid());

            Assert.Equal(order.Id, ex.OrderId);
            Assert.Equal(status, ex.CurrentStatus);
        }

    }
}
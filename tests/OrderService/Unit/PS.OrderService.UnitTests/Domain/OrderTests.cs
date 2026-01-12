using PS.OrderService.Domain.Entities;
using PS.OrderService.Domain.Enums;

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
            var order = new Order(Guid.NewGuid(), "address");
            var bookId = Guid.NewGuid();

            order.AddItem(bookId, 10m, 2);

            Assert.Single(order.Items);
            var item = order.Items.First();
            Assert.Equal(bookId, item.BookId);
            Assert.Equal(2, item.Quantity);
            Assert.Equal(20m, item.TotalPrice);
        }
    }
}

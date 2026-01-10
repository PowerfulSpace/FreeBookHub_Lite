using PS.OrderService.Domain.Entities;
using PS.OrderService.Domain.Enums;

namespace PS.OrderService.UnitTests.Domain
{
    public class OrderTests
    {
        [Fact]
        public void CreateOrder_ShouldInitializeCorrectly()
        {
            var userId = Guid.NewGuid();
            var address = "Some address";

            var order = new Order(userId, address);

            Assert.NotEqual(Guid.Empty, order.Id);
            Assert.Equal(userId, order.UserId);
            Assert.Equal(address, order.ShippingAddress);
            Assert.Equal(OrderStatus.New, order.Status);
            Assert.True(order.CreatedAt <= DateTime.UtcNow);
            Assert.Empty(order.Items);
        }
    }
}

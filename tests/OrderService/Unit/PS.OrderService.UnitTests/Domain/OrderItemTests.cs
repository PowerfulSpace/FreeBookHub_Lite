using PS.OrderService.Domain.Entities;

namespace PS.OrderService.UnitTests.Domain
{
    public class OrderItemTests
    {
        [Fact]
        public void CreateOrderItem_ShouldInitializeCorrectly()
        {
            var bookId = Guid.NewGuid();
            var price = 25m;
            var quantity = 2;

            var item = new OrderItem(bookId, price, quantity);

            Assert.Equal(bookId, item.BookId);
            Assert.Equal(price, item.UnitPrice);
            Assert.Equal(quantity, item.Quantity);
            Assert.Equal(50m, item.TotalPrice);
        }

    }
}

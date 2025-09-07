using PS.OrderService.Domain.Enums;
using PS.OrderService.Domain.Exceptions.Order;

namespace PS.OrderService.Domain.Entities
{
    public class Order
    {
        public Guid Id { get; private  set; }
        public Guid UserId { get; private set; }
        
        public string ShippingAddress { get; private set; } = string.Empty;
        public OrderStatus Status { get; private set; }
        public DateTime CreatedAt { get; private set; }


        private readonly List<OrderItem> _items = new();
        public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();


        public decimal TotalPrice => Items.Sum(i => i.UnitPrice * i.Quantity);

        protected Order()
        { 
        }

        public Order(Guid userId, string shippingAddress)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            CreatedAt = DateTime.UtcNow;
            Status = OrderStatus.New;
            ShippingAddress = shippingAddress;
        }

        public void AddItem(Guid bookId, decimal price, int quantity)
        {
            if (quantity <= 0)
                throw new InvalidOrderQuantityException(quantity);

            var existingItem = _items.FirstOrDefault(item => item.BookId == bookId);

            if (existingItem != null)
            {
                existingItem.UpdateQuantity(existingItem.Quantity + quantity);
            }
            else
            {
                _items.Add(new OrderItem(bookId, price, quantity));
            }
        }

        public void RemoveItem(Guid bookId)
        {
            var item = _items.FirstOrDefault(item => item.BookId == bookId);
            if (item != null)
            {
                _items.Remove(item);
            }
        }
        public void Cancel()
        {
            if (Status == OrderStatus.Shipped || Status == OrderStatus.Delivered || Status == OrderStatus.Cancelled)
                throw new CannotCancelOrderException(Id, Status);

            Status = OrderStatus.Cancelled;
        }

        public void MarkAsPaid()
        {
            if (Status != OrderStatus.New)
                throw new InvalidOrderPaymentStateException(Id, Status);

            Status = OrderStatus.Paid;
        }

    }
}

using PS.FreeBookHub_Lite.OrderService.Domain.Enums;

namespace PS.FreeBookHub_Lite.OrderService.Application.DTOs
{
    public class OrderResponse
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string ShippingAddress { get; set; } = string.Empty;
        public OrderStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public decimal TotalPrice { get; set; }

        public List<OrderItemDto> Items { get; set; } = new();
    }
}

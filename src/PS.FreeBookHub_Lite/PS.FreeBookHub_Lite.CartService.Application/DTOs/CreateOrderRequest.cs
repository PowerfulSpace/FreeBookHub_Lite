namespace PS.FreeBookHub_Lite.CartService.Application.DTOs
{
    public class CreateOrderRequest
    {
        public Guid UserId { get; set; }
        public string ShippingAddress { get; set; } = string.Empty;
        public List<OrderItemDto> Items { get; set; } = new();
    }
}

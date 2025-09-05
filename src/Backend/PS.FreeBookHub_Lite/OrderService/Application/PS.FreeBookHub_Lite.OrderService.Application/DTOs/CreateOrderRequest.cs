namespace PS.FreeBookHub_Lite.OrderService.Application.DTOs
{
    public class CreateOrderRequest
    {
        public Guid UserId { get; set; }
        public string ShippingAddress { get; set; } = string.Empty;

        public List<CreateOrderItemRequest> Items { get; set; } = new();
    }
}


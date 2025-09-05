namespace PS.FreeBookHub_Lite.OrderService.Application.DTOs
{
    public class CreateOrderItemRequest
    {
        public Guid BookId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}

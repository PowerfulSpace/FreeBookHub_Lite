namespace PS.OrderService.Application.DTOs
{
    public class CreateOrderItemRequest
    {
        public Guid BookId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}

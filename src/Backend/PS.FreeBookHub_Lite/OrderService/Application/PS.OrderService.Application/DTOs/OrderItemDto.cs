namespace PS.OrderService.Application.DTOs
{
    public class OrderItemDto
    {
        public Guid BookId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
    }
}

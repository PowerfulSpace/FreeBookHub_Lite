namespace PS.FreeBookHub_Lite.CartService.Application.DTOs.Order
{
    public class OrderItemDto
    {
        public Guid BookId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}

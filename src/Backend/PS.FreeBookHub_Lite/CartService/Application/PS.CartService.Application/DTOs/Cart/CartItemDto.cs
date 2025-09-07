namespace PS.CartService.Application.DTOs.Cart
{
    public class CartItemDto
    {
        public Guid BookId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
    }
}

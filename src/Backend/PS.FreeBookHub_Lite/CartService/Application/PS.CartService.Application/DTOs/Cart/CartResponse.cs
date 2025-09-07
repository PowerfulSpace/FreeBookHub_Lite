namespace PS.CartService.Application.DTOs.Cart
{
    public class CartResponse
    {
        public Guid UserId { get; set; }
        public List<CartItemDto> Items { get; set; } = new();
        public decimal TotalPrice { get; set; }
    }
}

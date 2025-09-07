namespace PS.CartService.Application.DTOs.Cart
{
    public class UpdateItemQuantityRequest
    {
        public Guid BookId { get; set; }
        public int Quantity { get; set; }
    }
}

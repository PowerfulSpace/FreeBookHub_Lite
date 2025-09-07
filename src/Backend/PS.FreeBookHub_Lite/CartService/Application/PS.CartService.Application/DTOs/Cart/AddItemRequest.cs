namespace PS.CartService.Application.DTOs.Cart
{
    public class AddItemRequest
    {
        public Guid BookId { get; set; }
        public int Quantity { get; set; }
    }
}

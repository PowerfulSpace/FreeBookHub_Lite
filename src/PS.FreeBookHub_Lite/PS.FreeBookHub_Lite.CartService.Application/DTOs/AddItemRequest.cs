namespace PS.FreeBookHub_Lite.CartService.Application.DTOs
{
    public class AddItemRequest
    {
        public Guid BookId { get; set; }
        public int Quantity { get; set; }
    }
}

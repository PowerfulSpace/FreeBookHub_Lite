namespace PS.FreeBookHub_Lite.CartService.Application.DTOs
{
    public class UpdateItemQuantityRequest
    {
        public Guid BookId { get; set; }
        public int Quantity { get; set; }
    }
}

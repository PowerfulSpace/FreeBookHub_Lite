namespace PS.FreeBookHub_Lite.CartService.Application.DTOs
{
    public class CheckoutRequest
    {
        public Guid UserId { get; set; }
        public string ShippingAddress { get; set; } = string.Empty;
    }
}

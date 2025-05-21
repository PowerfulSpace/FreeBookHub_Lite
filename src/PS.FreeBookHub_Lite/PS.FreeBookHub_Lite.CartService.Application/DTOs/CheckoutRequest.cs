namespace PS.FreeBookHub_Lite.CartService.Application.DTOs
{
    public record CheckoutRequest(Guid UserId, string ShippingAddress);
}

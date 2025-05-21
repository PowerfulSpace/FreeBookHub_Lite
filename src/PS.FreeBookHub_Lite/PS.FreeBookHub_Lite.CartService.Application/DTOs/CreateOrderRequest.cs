namespace PS.FreeBookHub_Lite.CartService.Application.DTOs
{
    public record CreateOrderRequest(
    Guid UserId,
    string ShippingAddress,
    List<OrderItemDto> Items
);
}

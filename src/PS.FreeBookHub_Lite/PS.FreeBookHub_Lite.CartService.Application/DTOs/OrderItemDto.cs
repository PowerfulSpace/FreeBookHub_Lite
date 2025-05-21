namespace PS.FreeBookHub_Lite.CartService.Application.DTOs
{
    public record OrderItemDto(
     Guid BookId,
     int Quantity,
     decimal UnitPrice
 );
}

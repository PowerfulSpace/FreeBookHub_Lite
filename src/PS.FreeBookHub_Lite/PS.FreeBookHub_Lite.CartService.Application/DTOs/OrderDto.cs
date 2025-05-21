namespace PS.FreeBookHub_Lite.CartService.Application.DTOs
{
    public record OrderDto(
        Guid Id,
        Guid UserId,
        string Status,
        decimal TotalPrice
        );
}

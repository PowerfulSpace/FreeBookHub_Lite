namespace PS.FreeBookHub_Lite.OrderService.Common.Events
{
    public record OrderCreatedEvent(
        Guid OrderId,
        Guid UserId,
        decimal Amount,
        DateTime CreatedAt);
}

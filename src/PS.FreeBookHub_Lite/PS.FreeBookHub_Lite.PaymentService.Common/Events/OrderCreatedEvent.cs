namespace PS.FreeBookHub_Lite.PaymentService.Common.Events
{
    public record OrderCreatedEvent(
       Guid OrderId,
       Guid UserId,
       decimal Amount,
       DateTime CreatedAt);
}

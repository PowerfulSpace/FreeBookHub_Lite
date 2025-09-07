namespace PS.PaymentService.Common.Events
{
    public record OrderCreatedEvent(
       Guid OrderId,
       Guid UserId,
       decimal Amount,
       DateTime CreatedAt);
}

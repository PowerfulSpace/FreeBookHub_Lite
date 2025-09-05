namespace PS.FreeBookHub_Lite.OrderService.Common.Events
{
    public record PaymentCompletedEvent(
     Guid OrderId,
     Guid PaymentId,
     DateTime CompletedAt);
}

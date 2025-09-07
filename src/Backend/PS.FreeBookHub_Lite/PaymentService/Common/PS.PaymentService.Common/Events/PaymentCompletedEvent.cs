namespace PS.PaymentService.Common.Events
{
    public record PaymentCompletedEvent(
      Guid OrderId,
      Guid PaymentId,
      DateTime CompletedAt);
}

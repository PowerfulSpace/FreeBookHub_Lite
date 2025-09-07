namespace PS.PaymentService.Application.Interfaces.Redis
{
    public interface IEventDeduplicationService
    {
        Task<bool> IsDuplicateAsync(string key, TimeSpan ttl, CancellationToken ct = default);
    }
}

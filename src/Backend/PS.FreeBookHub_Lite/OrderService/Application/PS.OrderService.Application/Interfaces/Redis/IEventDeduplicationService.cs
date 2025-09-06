namespace PS.FreeBookHub_Lite.OrderService.Application.Interfaces.Redis
{
    public interface IEventDeduplicationService
    {
        Task<bool> IsDuplicateAsync(string key, TimeSpan ttl, CancellationToken ct = default);
    }
}

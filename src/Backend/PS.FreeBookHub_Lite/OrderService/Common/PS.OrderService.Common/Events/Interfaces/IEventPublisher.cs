namespace PS.OrderService.Common.Events.Interfaces
{

    public interface IEventPublisher
    {
        Task PublishAsync<TEvent>(TEvent @event, string routingKey, CancellationToken cancellationToken = default);
    }
}
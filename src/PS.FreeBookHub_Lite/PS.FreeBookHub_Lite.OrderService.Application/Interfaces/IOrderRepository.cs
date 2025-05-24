using PS.FreeBookHub_Lite.OrderService.Domain.Entities;

namespace PS.FreeBookHub_Lite.OrderService.Application.Interfaces
{
    public interface IOrderRepository
    {
        Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken, bool asNoTracking = false);
        Task<IEnumerable<Order>> GetAllByUserIdAsync(Guid userId, CancellationToken cancellationToken);
        Task AddAsync(Order order, CancellationToken cancellationToken);
        Task UpdateAsync(Order order, CancellationToken cancellationToken);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken);
    }
}

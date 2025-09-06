using PS.FreeBookHub_Lite.CartService.Domain.Entities;

namespace PS.FreeBookHub_Lite.CartService.Application.Interfaces
{
    public interface ICartRepository
    {
        Task<Cart?> GetCartAsync(Guid userId, CancellationToken cancellationToken, bool asNoTracking = false);
        Task AddAsync(Cart cart, CancellationToken cancellationToken);
        Task UpdateAsync(Cart cart, CancellationToken cancellationToken);
        Task DeleteAsync(Guid userId, CancellationToken cancellationToken);
    }
}

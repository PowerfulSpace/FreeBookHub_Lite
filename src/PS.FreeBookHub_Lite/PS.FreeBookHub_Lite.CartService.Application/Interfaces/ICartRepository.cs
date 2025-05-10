using PS.FreeBookHub_Lite.CartService.Domain.Entities;

namespace PS.FreeBookHub_Lite.CartService.Application.Interfaces
{
    public interface ICartRepository
    {
        Task<Cart?> GetCartAsync(Guid userId, bool asNoTracking = false);
        Task AddAsync(Cart cart);
        Task UpdateAsync(Cart cart);
        Task DeleteAsync(Guid userId);
    }
}

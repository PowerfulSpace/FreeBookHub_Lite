using PS.FreeBookHub_Lite.CartService.Domain.Entities;

namespace PS.FreeBookHub_Lite.CartService.Application.Interfaces
{
    public interface ICartRepository
    {
        Task<Cart?> GetCartAsync(Guid userId);
        Task AddOrUpdateItemAsync(Guid userId, Guid bookId, int quantity);
        Task RemoveItemAsync(Guid userId, Guid bookId);
        Task ClearCartAsync(Guid userId);
    }
}

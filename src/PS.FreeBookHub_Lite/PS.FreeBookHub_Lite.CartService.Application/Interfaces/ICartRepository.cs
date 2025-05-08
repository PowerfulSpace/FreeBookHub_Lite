using PS.FreeBookHub_Lite.CartService.Domain.Entities;

namespace PS.FreeBookHub_Lite.CartService.Application.Interfaces
{
    public interface ICartRepository
    {
        Task<Cart?> GetCartAsync(Guid userId);
        Task UpdateQuantityAsync(Guid userId, Guid bookId, int quantity);
        Task AddOrUpdateItemAsync(Guid userId, Guid bookId, int quantity, decimal price);
        Task RemoveItemAsync(Guid userId, Guid bookId);
        Task ClearCartAsync(Guid userId);
        Task AddOrUpdateItemAsync(Guid userId, Guid bookId, int quantity);
    }
}

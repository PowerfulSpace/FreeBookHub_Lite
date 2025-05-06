using PS.FreeBookHub_Lite.CartService.Application.DTOs;

namespace PS.FreeBookHub_Lite.CartService.Application.Services.Interfaces
{
    public interface ICartBookService
    {
        Task<CartDto> GetCartAsync(Guid userId);
        Task AddItemAsync(Guid userId, AddItemRequest request);
        Task UpdateItemQuantityAsync(Guid userId, UpdateItemQuantityRequest request);
        Task RemoveItemAsync(Guid userId, Guid bookId);
        Task ClearCartAsync(Guid userId);
    }
}

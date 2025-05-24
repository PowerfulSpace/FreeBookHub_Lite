using PS.FreeBookHub_Lite.CartService.Application.DTOs.Cart;
using PS.FreeBookHub_Lite.CartService.Application.DTOs.Order;

namespace PS.FreeBookHub_Lite.CartService.Application.Services.Interfaces
{
    public interface ICartBookService
    {
        Task<CartResponse> GetCartAsync(Guid userId);
        Task AddItemAsync(Guid userId, AddItemRequest request);
        Task UpdateItemQuantityAsync(Guid userId, UpdateItemQuantityRequest request);
        Task RemoveItemAsync(Guid userId, Guid bookId);
        Task ClearCartAsync(Guid userId);

        Task<OrderResponse> CheckoutAsync(Guid userId, string shippingAddress);
    }
}

using PS.FreeBookHub_Lite.CartService.Application.DTOs.Cart;
using PS.FreeBookHub_Lite.CartService.Application.DTOs.Order;

namespace PS.FreeBookHub_Lite.CartService.Application.Services.Interfaces
{
    public interface ICartBookService
    {
        Task<CartResponse> GetCartAsync(Guid userId, CancellationToken cancellationToken);
        Task AddItemAsync(Guid userId, AddItemRequest request, CancellationToken cancellationToken);
        Task UpdateItemQuantityAsync(Guid userId, UpdateItemQuantityRequest request, CancellationToken cancellationToken);
        Task RemoveItemAsync(Guid userId, Guid bookId, CancellationToken cancellationToken);
        Task ClearCartAsync(Guid userId, CancellationToken cancellationToken);

        Task<OrderResponse> CheckoutAsync(Guid userId, string shippingAddress, CancellationToken cancellationToken);
    }
}

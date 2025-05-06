using Mapster;
using PS.FreeBookHub_Lite.CartService.Application.DTOs;
using PS.FreeBookHub_Lite.CartService.Application.Interfaces;
using PS.FreeBookHub_Lite.CartService.Application.Services.Interfaces;
using PS.FreeBookHub_Lite.CartService.Domain.Entities;

namespace PS.FreeBookHub_Lite.CartService.Application.Services
{
    public class CartBookService : ICartBookService
    {
        private readonly ICartRepository _cartRepository;

        public CartBookService(ICartRepository cartRepository)
        {
            _cartRepository = cartRepository;
        }

        public async Task<CartDto> GetCartAsync(Guid userId)
        {
            var cart = await _cartRepository.GetCartAsync(userId)
                       ?? new Cart { UserId = userId, Items = new List<CartItem>() };

            return cart.Adapt<CartDto>();
        }

        public async Task AddItemAsync(Guid userId, AddItemRequest request)
        {
            await _cartRepository.AddOrUpdateItemAsync(userId, request.BookId, request.Quantity);
        }

        public async Task UpdateItemQuantityAsync(Guid userId, UpdateItemQuantityRequest request)
        {
            await _cartRepository.AddOrUpdateItemAsync(userId, request.BookId, request.Quantity);
        }

        public async Task RemoveItemAsync(Guid userId, Guid bookId)
        {
            await _cartRepository.RemoveItemAsync(userId, bookId);
        }

        public async Task ClearCartAsync(Guid userId)
        {
            await _cartRepository.ClearCartAsync(userId);
        }
    }
}

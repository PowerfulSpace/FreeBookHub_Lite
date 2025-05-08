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
        private readonly IBookCatalogClient _bookCatalogClient;

        public CartBookService(
            ICartRepository cartRepository,
            IBookCatalogClient bookCatalogClient
            )
        {
            _cartRepository = cartRepository;
            _bookCatalogClient = bookCatalogClient;
        }

        public async Task<CartDto> GetCartAsync(Guid userId)
        {
            var cart = await _cartRepository.GetCartAsync(userId)
                       ?? new Cart(userId);

            return cart.Adapt<CartDto>();
        }

        public async Task AddItemAsync(Guid userId, AddItemRequest request)
        {

            var cart = await _cartRepository.GetCartAsync(userId)
                   ?? new Cart(userId);

            var price = await _bookCatalogClient.GetBookPriceAsync(request.BookId);
            if (price is null)
                throw new InvalidOperationException("Book not found or price unavailable.");

            cart.AddItem(request.BookId, request.Quantity, price.Value);

            await _cartRepository.AddOrUpdateItemAsync(userId, request.BookId, request.Quantity);
        }

        public async Task UpdateItemQuantityAsync(Guid userId, UpdateItemQuantityRequest request)
        {
            await _cartRepository.UpdateQuantityAsync(userId, request.BookId, request.Quantity);
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

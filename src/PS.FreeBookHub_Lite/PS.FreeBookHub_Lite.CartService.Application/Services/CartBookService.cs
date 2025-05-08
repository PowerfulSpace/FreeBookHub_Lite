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
            var existingCart = await _cartRepository.GetCartAsync(userId);
            var cart = existingCart ?? new Cart(userId);

            var price = await _bookCatalogClient.GetBookPriceAsync(request.BookId);
            if (price is null)
                throw new Exception("Book not found or price unavailable.");

            cart.AddItem(request.BookId, request.Quantity, price.Value);

            if (existingCart is null)
                await _cartRepository.AddAsync(cart);
            else
                await _cartRepository.UpdateAsync(cart);
        }

        public async Task UpdateItemQuantityAsync(Guid userId, UpdateItemQuantityRequest request)
        {
            var cart = await _cartRepository.GetCartAsync(userId)
                       ?? throw new Exception("Cart not found.");

            cart.UpdateQuantity(request.BookId, request.Quantity);

            await _cartRepository.UpdateAsync(cart);
        }

        public async Task RemoveItemAsync(Guid userId, Guid bookId)
        {
            var cart = await _cartRepository.GetCartAsync(userId)
                      ?? throw new Exception("Cart not found.");

            cart.RemoveItem(bookId);

            await _cartRepository.UpdateAsync(cart);
        }

        public async Task ClearCartAsync(Guid userId)
        {
            var cart = await _cartRepository.GetCartAsync(userId)
                      ?? throw new Exception("Cart not found.");

            cart.Clear();
            await _cartRepository.UpdateAsync(cart);
        }
    }
}

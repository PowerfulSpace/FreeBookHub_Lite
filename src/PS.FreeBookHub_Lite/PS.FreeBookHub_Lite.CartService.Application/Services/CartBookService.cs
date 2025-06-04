using Mapster;
using PS.FreeBookHub_Lite.CartService.Application.Clients;
using PS.FreeBookHub_Lite.CartService.Application.DTOs.Cart;
using PS.FreeBookHub_Lite.CartService.Application.DTOs.Order;
using PS.FreeBookHub_Lite.CartService.Application.Interfaces;
using PS.FreeBookHub_Lite.CartService.Application.Services.Interfaces;
using PS.FreeBookHub_Lite.CartService.Domain.Entities;

namespace PS.FreeBookHub_Lite.CartService.Application.Services
{
    public class CartBookService : ICartBookService
    {

        private readonly ICartRepository _cartRepository;
        private readonly IBookCatalogClient _bookCatalogClient;
        private readonly IOrderServiceClient _orderServiceClient;

        public CartBookService(
            ICartRepository cartRepository,
            IBookCatalogClient bookCatalogClient,
            IOrderServiceClient orderServiceClient
            )
        {
            _cartRepository = cartRepository;
            _bookCatalogClient = bookCatalogClient;
            _orderServiceClient = orderServiceClient;
        }

        public async Task<CartResponse> GetCartAsync(Guid userId, CancellationToken cancellationToken)
        {
            var cart = await _cartRepository.GetCartAsync(userId, cancellationToken, true)
                      ?? new Cart(userId);

            return cart.Adapt<CartResponse>();
        }

        public async Task AddItemAsync(Guid userId, AddItemRequest request, CancellationToken cancellationToken)
        {
            var existingCart = await _cartRepository.GetCartAsync(userId, cancellationToken);
            var cart = existingCart ?? new Cart(userId);

            var price = await _bookCatalogClient.GetBookPriceAsync(request.BookId, cancellationToken);
            if (price is null)
                throw new Exception("Book not found or price unavailable.");

            cart.AddItem(request.BookId, request.Quantity, price.Value);

            if (existingCart is null)
                await _cartRepository.AddAsync(cart, cancellationToken);
            else
                await _cartRepository.UpdateAsync(cart, cancellationToken);
        }

        public async Task UpdateItemQuantityAsync(Guid userId, UpdateItemQuantityRequest request, CancellationToken cancellationToken)
        {
            var cart = await _cartRepository.GetCartAsync(userId, cancellationToken)
                       ?? throw new Exception("Cart not found.");

            cart.UpdateQuantity(request.BookId, request.Quantity);

            await _cartRepository.UpdateAsync(cart, cancellationToken);
        }

        public async Task RemoveItemAsync(Guid userId, Guid bookId, CancellationToken cancellationToken)
        {
            var cart = await _cartRepository.GetCartAsync(userId, cancellationToken)
                      ?? throw new Exception("Cart not found.");

            cart.RemoveItem(bookId);

            await _cartRepository.UpdateAsync(cart, cancellationToken);
        }

        public async Task ClearCartAsync(Guid userId, CancellationToken cancellationToken)
        {
            var cart = await _cartRepository.GetCartAsync(userId, cancellationToken)
                      ?? throw new Exception("Cart not found.");

            cart.Clear();
            await _cartRepository.UpdateAsync(cart, cancellationToken);
        }

        public async Task<OrderResponse> CheckoutAsync(Guid userId, string shippingAddress, CancellationToken cancellationToken)
        {
            var cart = await _cartRepository.GetCartAsync(userId, cancellationToken)
                       ?? throw new Exception("Cart is empty");

            if (!cart.Items.Any())
                throw new InvalidOperationException("Unable to place an order: cart is empty");

            // Подготовка запроса для OrderService
            var orderRequest = new CreateOrderRequest
            {
                UserId = userId,
                ShippingAddress = shippingAddress,
                Items = cart.Items.Select(i => new OrderItemDto
                {
                    BookId = i.BookId,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice // Берём цену из корзины (уже проверенную через CatalogService)
                }).ToList()
            };

            // Создание заказа
            var order = await _orderServiceClient.CreateOrderAsync(orderRequest, cancellationToken);

            // Очистка корзины после успешного оформления
            await ClearCartAsync(userId, cancellationToken);

            return order;
        }
    }
}

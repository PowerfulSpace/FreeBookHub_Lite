using Mapster;
using Microsoft.Extensions.Logging;
using PS.FreeBookHub_Lite.CartService.Application.Clients;
using PS.FreeBookHub_Lite.CartService.Application.DTOs.Cart;
using PS.FreeBookHub_Lite.CartService.Application.DTOs.Order;
using PS.FreeBookHub_Lite.CartService.Application.Interfaces;
using PS.FreeBookHub_Lite.CartService.Application.Services.Interfaces;
using PS.FreeBookHub_Lite.CartService.Common;
using PS.FreeBookHub_Lite.CartService.Domain.Entities;
using PS.FreeBookHub_Lite.CartService.Domain.Exceptions;

namespace PS.FreeBookHub_Lite.CartService.Application.Services
{
    public class CartBookService : ICartBookService
    {

        private readonly ICartRepository _cartRepository;
        private readonly IBookCatalogClient _bookCatalogClient;
        private readonly IOrderServiceClient _orderServiceClient;
        private readonly ILogger<CartBookService> _logger;

        public CartBookService(
            ICartRepository cartRepository,
            IBookCatalogClient bookCatalogClient,
            IOrderServiceClient orderServiceClient,
            ILogger<CartBookService> logger
            )
        {
            _cartRepository = cartRepository;
            _bookCatalogClient = bookCatalogClient;
            _orderServiceClient = orderServiceClient;
            _logger = logger;
        }

        public async Task<CartResponse> GetCartAsync(Guid userId, CancellationToken cancellationToken)
        {
            _logger.LogInformation(LoggerMessages.GetCartStarted, userId);

            var cart = await _cartRepository.GetCartAsync(userId, cancellationToken, true)
                      ?? new Cart(userId);

            _logger.LogInformation(LoggerMessages.GetCartSuccess, userId);

            return cart.Adapt<CartResponse>();
        }

        public async Task AddItemAsync(Guid userId, AddItemRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation(LoggerMessages.AddItemStarted, userId, request.BookId, request.Quantity);

            var existingCart = await _cartRepository.GetCartAsync(userId, cancellationToken);
            var cart = existingCart ?? new Cart(userId);

            var price = await _bookCatalogClient.GetBookPriceAsync(request.BookId, cancellationToken);
            if (price is null)
            {
                throw new BookNotFoundException(request.BookId);
            }

            cart.AddItem(request.BookId, request.Quantity, price.Value);

            if (existingCart is null)
            {
                await _cartRepository.AddAsync(cart, cancellationToken);
                _logger.LogInformation(LoggerMessages.CartCreated, userId);
            }
            else
            {
                await _cartRepository.UpdateAsync(cart, cancellationToken);
            }

            _logger.LogInformation(LoggerMessages.AddItemSuccess, userId, request.BookId);
        }

        public async Task UpdateItemQuantityAsync(Guid userId, UpdateItemQuantityRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation(LoggerMessages.UpdateQuantityStarted, userId, request.BookId, request.Quantity);

            var cart = await _cartRepository.GetCartAsync(userId, cancellationToken)
                       ?? throw new CartNotFoundException(userId);

            cart.UpdateQuantity(request.BookId, request.Quantity);

            await _cartRepository.UpdateAsync(cart, cancellationToken);

            _logger.LogInformation(LoggerMessages.UpdateQuantitySuccess, userId, request.BookId);
        }

        public async Task RemoveItemAsync(Guid userId, Guid bookId, CancellationToken cancellationToken)
        {
            _logger.LogInformation(LoggerMessages.RemoveItemStarted, userId, bookId);

            var cart = await _cartRepository.GetCartAsync(userId, cancellationToken)
                      ?? throw new CartNotFoundException(userId);

            cart.RemoveItem(bookId);

            await _cartRepository.UpdateAsync(cart, cancellationToken);

            _logger.LogInformation(LoggerMessages.RemoveItemSuccess, userId, bookId);
        }

        public async Task ClearCartAsync(Guid userId, CancellationToken cancellationToken)
        {
            _logger.LogInformation(LoggerMessages.ClearCartStarted, userId);

            var cart = await _cartRepository.GetCartAsync(userId, cancellationToken)
                      ?? throw new CartNotFoundException(userId);

            cart.Clear();
            await _cartRepository.UpdateAsync(cart, cancellationToken);

            _logger.LogInformation(LoggerMessages.ClearCartSuccess, userId);
        }

        public async Task<OrderResponse> CheckoutAsync(Guid userId, string shippingAddress, CancellationToken cancellationToken)
        {
            _logger.LogInformation(LoggerMessages.CheckoutStarted, userId);

            var cart = await _cartRepository.GetCartAsync(userId, cancellationToken)
                       ?? throw new CartNotFoundException(userId);

            if (!cart.Items.Any())
                throw new EmptyCartException(userId);

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

            _logger.LogInformation(LoggerMessages.CheckoutSuccess, userId, order.Id);

            return order;
        }
    }
}

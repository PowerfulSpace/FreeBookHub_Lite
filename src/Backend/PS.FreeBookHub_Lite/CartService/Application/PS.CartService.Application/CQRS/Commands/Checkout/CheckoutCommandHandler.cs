using MediatR;
using Microsoft.Extensions.Logging;
using PS.CartService.Application.Clients;
using PS.CartService.Application.DTOs.Order;
using PS.CartService.Application.Interfaces;
using PS.CartService.Common.Logging;
using PS.CartService.Domain.Exceptions.Cart;

namespace PS.CartService.Application.CQRS.Commands.Checkout
{
    public class CheckoutCommandHandler : IRequestHandler<CheckoutCommand, OrderResponse>
    {
        private readonly ICartRepository _cartRepository;
        private readonly IOrderServiceClient _orderServiceClient;
        private readonly ILogger<CheckoutCommandHandler> _logger;

        public CheckoutCommandHandler(
            ICartRepository cartRepository,
            IOrderServiceClient orderServiceClient,
            ILogger<CheckoutCommandHandler> logger)
        {
            _cartRepository = cartRepository;
            _orderServiceClient = orderServiceClient;
            _logger = logger;
        }

        public async Task<OrderResponse> Handle(CheckoutCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation(LoggerMessages.CheckoutStarted, request.UserId);

            var cart = await _cartRepository.GetCartAsync(request.UserId, cancellationToken)
                       ?? throw new CartNotFoundException(request.UserId);

            if (!cart.Items.Any())
                throw new EmptyCartException(request.UserId);

            var orderRequest = new CreateOrderRequest
            {
                UserId = request.UserId,
                ShippingAddress = request.ShippingAddress,
                Items = cart.Items.Select(i => new OrderItemDto
                {
                    BookId = i.BookId,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList()
            };

            var order = await _orderServiceClient.CreateOrderAsync(orderRequest, cancellationToken);

            _logger.LogInformation(LoggerMessages.ClearCartStarted, request.UserId);
            cart.Clear();
            await _cartRepository.UpdateAsync(cart, cancellationToken);
            _logger.LogInformation(LoggerMessages.ClearCartSuccess, request.UserId);

            _logger.LogInformation(LoggerMessages.CheckoutSuccess, request.UserId, order.Id);

            return order;
        }
    }
}

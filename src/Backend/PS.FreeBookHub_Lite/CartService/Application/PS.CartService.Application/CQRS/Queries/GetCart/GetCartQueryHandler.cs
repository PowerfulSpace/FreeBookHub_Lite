using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;
using PS.CartService.Application.DTOs.Cart;
using PS.CartService.Application.Interfaces;
using PS.CartService.Common.Logging;
using PS.CartService.Domain.Entities;

namespace PS.CartService.Application.CQRS.Queries.GetCart
{
    public class GetCartQueryHandler : IRequestHandler<GetCartQuery, CartResponse>
    {
        private readonly ICartRepository _cartRepository;
        private readonly ILogger<GetCartQueryHandler> _logger;

        public GetCartQueryHandler(
            ICartRepository cartRepository,
            ILogger<GetCartQueryHandler> logger)
        {
            _cartRepository = cartRepository;
            _logger = logger;
        }

        public async Task<CartResponse> Handle(GetCartQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation(LoggerMessages.GetCartStarted, request.UserId);

            var cart = await _cartRepository.GetCartAsync(request.UserId, cancellationToken, asNoTracking: true)
                       ?? new Cart(request.UserId);

            _logger.LogInformation(LoggerMessages.GetCartSuccess, request.UserId);

            return cart.Adapt<CartResponse>();
        }
    }
}

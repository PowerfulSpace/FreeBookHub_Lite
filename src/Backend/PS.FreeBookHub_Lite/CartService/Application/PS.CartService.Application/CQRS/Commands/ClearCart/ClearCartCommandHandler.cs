using MediatR;
using Microsoft.Extensions.Logging;
using PS.CartService.Application.Interfaces;
using PS.CartService.Common.Logging;
using PS.CartService.Domain.Exceptions.Cart;

namespace PS.CartService.Application.CQRS.Commands.ClearCart
{
    public class ClearCartCommandHandler : IRequestHandler<ClearCartCommand, Unit>
    {
        private readonly ICartRepository _cartRepository;
        private readonly ILogger<ClearCartCommandHandler> _logger;

        public ClearCartCommandHandler(
            ICartRepository cartRepository,
            ILogger<ClearCartCommandHandler> logger)
        {
            _cartRepository = cartRepository;
            _logger = logger;
        }

        public async Task<Unit> Handle(ClearCartCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation(LoggerMessages.ClearCartStarted, request.UserId);

            var cart = await _cartRepository.GetCartAsync(request.UserId, cancellationToken)
                       ?? throw new CartNotFoundException(request.UserId);

            cart.Clear();
            await _cartRepository.UpdateAsync(cart, cancellationToken);

            _logger.LogInformation(LoggerMessages.ClearCartSuccess, request.UserId);

            return Unit.Value;
        }
    }
}

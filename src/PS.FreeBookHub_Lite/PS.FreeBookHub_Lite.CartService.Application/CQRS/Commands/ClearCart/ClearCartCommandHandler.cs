using MediatR;
using Microsoft.Extensions.Logging;
using PS.FreeBookHub_Lite.CartService.Application.Interfaces;
using PS.FreeBookHub_Lite.CartService.Common;
using PS.FreeBookHub_Lite.CartService.Domain.Exceptions.Cart;

namespace PS.FreeBookHub_Lite.CartService.Application.CQRS.Commands.ClearCart
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

using MediatR;
using Microsoft.Extensions.Logging;
using PS.FreeBookHub_Lite.CartService.Application.Interfaces;
using PS.FreeBookHub_Lite.CartService.Common.Logging;
using PS.FreeBookHub_Lite.CartService.Domain.Exceptions.Cart;

namespace PS.FreeBookHub_Lite.CartService.Application.CQRS.Commands.UpdateItemQuantity
{
    public class UpdateItemQuantityCommandHandler : IRequestHandler<UpdateItemQuantityCommand, Unit>
    {
        private readonly ICartRepository _cartRepository;
        private readonly ILogger<UpdateItemQuantityCommandHandler> _logger;

        public UpdateItemQuantityCommandHandler(
            ICartRepository cartRepository,
            ILogger<UpdateItemQuantityCommandHandler> logger)
        {
            _cartRepository = cartRepository;
            _logger = logger;
        }

        public async Task<Unit> Handle(UpdateItemQuantityCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation(LoggerMessages.UpdateQuantityStarted, request.UserId, request.BookId, request.Quantity);

            var cart = await _cartRepository.GetCartAsync(request.UserId, cancellationToken)
                       ?? throw new CartNotFoundException(request.UserId);

            cart.UpdateQuantity(request.BookId, request.Quantity);

            await _cartRepository.UpdateAsync(cart, cancellationToken);

            _logger.LogInformation(LoggerMessages.UpdateQuantitySuccess, request.UserId, request.BookId);

            return Unit.Value;
        }
    }
}

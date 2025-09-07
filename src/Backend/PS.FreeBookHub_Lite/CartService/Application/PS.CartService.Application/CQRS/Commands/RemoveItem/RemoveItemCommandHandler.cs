using MediatR;
using Microsoft.Extensions.Logging;
using PS.CartService.Application.Interfaces;
using PS.CartService.Common.Logging;
using PS.CartService.Domain.Exceptions.Cart;

namespace PS.CartService.Application.CQRS.Commands.RemoveItem
{
    public class RemoveItemCommandHandler : IRequestHandler<RemoveItemCommand, Unit>
    {
        private readonly ICartRepository _cartRepository;
        private readonly ILogger<RemoveItemCommandHandler> _logger;

        public RemoveItemCommandHandler(
            ICartRepository cartRepository,
            ILogger<RemoveItemCommandHandler> logger)
        {
            _cartRepository = cartRepository;
            _logger = logger;
        }

        public async Task<Unit> Handle(RemoveItemCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation(LoggerMessages.RemoveItemStarted, request.UserId, request.BookId);

            var cart = await _cartRepository.GetCartAsync(request.UserId, cancellationToken)
                       ?? throw new CartNotFoundException(request.UserId);

            cart.RemoveItem(request.BookId);

            await _cartRepository.UpdateAsync(cart, cancellationToken);

            _logger.LogInformation(LoggerMessages.RemoveItemSuccess, request.UserId, request.BookId);

            return Unit.Value;
        }
    }
}

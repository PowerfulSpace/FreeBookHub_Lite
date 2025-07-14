using MediatR;
using Microsoft.Extensions.Logging;
using PS.FreeBookHub_Lite.CartService.Application.Clients;
using PS.FreeBookHub_Lite.CartService.Application.Interfaces;
using PS.FreeBookHub_Lite.CartService.Common.Logging;
using PS.FreeBookHub_Lite.CartService.Domain.Entities;
using PS.FreeBookHub_Lite.CartService.Domain.Exceptions.Cart;

namespace PS.FreeBookHub_Lite.CartService.Application.CQRS.Commands.AddItem
{
    public class AddItemCommandHandler : IRequestHandler<AddItemCommand, Unit>
    {
        private readonly ICartRepository _cartRepository;
        private readonly IBookCatalogClient _bookCatalogClient;
        private readonly ILogger<AddItemCommandHandler> _logger;

        public AddItemCommandHandler(
            ICartRepository cartRepository,
            IBookCatalogClient bookCatalogClient,
            ILogger<AddItemCommandHandler> logger)
        {
            _cartRepository = cartRepository;
            _bookCatalogClient = bookCatalogClient;
            _logger = logger;
        }

        public async Task<Unit> Handle(AddItemCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation(LoggerMessages.AddItemStarted, request.UserId, request.BookId, request.Quantity);

            var existingCart = await _cartRepository.GetCartAsync(request.UserId, cancellationToken);
            var cart = existingCart ?? new Cart(request.UserId);

            var price = await _bookCatalogClient.GetBookPriceAsync(request.BookId, cancellationToken);
            if (price is null)
            {
                throw new BookNotFoundException(request.BookId);
            }

            cart.AddItem(request.BookId, request.Quantity, price.Value);

            if (existingCart is null)
            {
                await _cartRepository.AddAsync(cart, cancellationToken);
                _logger.LogInformation(LoggerMessages.CartCreated, request.UserId);
            }
            else
            {
                await _cartRepository.UpdateAsync(cart, cancellationToken);
            }

            _logger.LogInformation(LoggerMessages.AddItemSuccess, request.UserId, request.BookId);

            return Unit.Value;
        }
    }
}

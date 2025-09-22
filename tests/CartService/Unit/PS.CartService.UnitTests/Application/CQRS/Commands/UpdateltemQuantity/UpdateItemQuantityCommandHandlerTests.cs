using Microsoft.Extensions.Logging;
using Moq;
using PS.CartService.Application.CQRS.Commands.UpdateItemQuantity;
using PS.CartService.Application.Interfaces;

namespace PS.CartService.UnitTests.Application.CQRS.Commands.UpdateltemQuantity
{
    public class UpdateItemQuantityCommandHandlerTests
    {
        private readonly Mock<ICartRepository> _cartRepoMock = new();
        private readonly Mock<ILogger<UpdateItemQuantityCommandHandler>> _loggerMock = new();

        private UpdateItemQuantityCommandHandler CreateHandler() =>
            new(_cartRepoMock.Object, _loggerMock.Object);
    }
}

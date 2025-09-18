using Microsoft.Extensions.Logging;
using Moq;
using PS.CartService.Application.CQRS.Commands.ClearCart;
using PS.CartService.Application.Interfaces;
using PS.CartService.Domain.Entities;
using PS.CartService.Domain.Exceptions.Cart;

namespace PS.CartService.UnitTests.Application.CQRS.Commands.ClearCart
{
    public class ClearCartCommandHandlerTests
    {
        private readonly Mock<ICartRepository> _cartRepoMock = new();
        private readonly Mock<ILogger<ClearCartCommandHandler>> _loggerMock = new();

        private ClearCartCommandHandler CreateHandler() =>
            new(_cartRepoMock.Object, _loggerMock.Object);


        [Fact]
        public async Task Handle_CartNotFound_ShouldThrow()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var command = new ClearCartCommand(userId);

            _cartRepoMock.Setup(r => r.GetCartAsync(userId, It.IsAny<CancellationToken>(), It.IsAny<bool>()))
                         .ReturnsAsync((Cart?)null);

            var handler = CreateHandler();

            // Act + Assert
            await Assert.ThrowsAsync<CartNotFoundException>(() => handler.Handle(command, default));
        }
    }
}

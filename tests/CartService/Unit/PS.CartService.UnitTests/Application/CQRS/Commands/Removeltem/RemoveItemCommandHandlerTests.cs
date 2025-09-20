using Microsoft.Extensions.Logging;
using Moq;
using PS.CartService.Application.CQRS.Commands.RemoveItem;
using PS.CartService.Application.Interfaces;
using PS.CartService.Domain.Entities;
using PS.CartService.Domain.Exceptions.Cart;

namespace PS.CartService.UnitTests.Application.CQRS.Commands.Removeltem
{
    public class RemoveItemCommandHandlerTests
    {
        private readonly Mock<ICartRepository> _cartRepoMock = new();
        private readonly Mock<ILogger<RemoveItemCommandHandler>> _loggerMock = new();

        private RemoveItemCommandHandler CreateHandler() =>
            new(_cartRepoMock.Object, _loggerMock.Object);

        [Fact]
        public async Task Handle_CartNotFound_ShouldThrow()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var bookId = Guid.NewGuid();
            var command = new RemoveItemCommand(userId, bookId);

            _cartRepoMock.Setup(r => r.GetCartAsync(userId, It.IsAny<CancellationToken>(), It.IsAny<bool>()))
                         .ReturnsAsync((Cart?)null);

            var handler = CreateHandler();

            // Act + Assert
            await Assert.ThrowsAsync<CartNotFoundException>(() => handler.Handle(command, default));
        }

    }
}

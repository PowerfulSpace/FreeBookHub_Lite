using Microsoft.Extensions.Logging;
using Moq;
using PS.CartService.Application.CQRS.Commands.UpdateItemQuantity;
using PS.CartService.Application.Interfaces;
using PS.CartService.Domain.Entities;
using PS.CartService.Domain.Exceptions.Cart;

namespace PS.CartService.UnitTests.Application.CQRS.Commands.UpdateltemQuantity
{
    public class UpdateItemQuantityCommandHandlerTests
    {
        private readonly Mock<ICartRepository> _cartRepoMock = new();
        private readonly Mock<ILogger<UpdateItemQuantityCommandHandler>> _loggerMock = new();

        private UpdateItemQuantityCommandHandler CreateHandler() =>
            new(_cartRepoMock.Object, _loggerMock.Object);


        [Fact]
        public async Task Handle_CartNotFound_ShouldThrow()
        {
            var userId = Guid.NewGuid();
            var bookId = Guid.NewGuid();
            var command = new UpdateItemQuantityCommand(userId, bookId, 5);

            _cartRepoMock.Setup(r => r.GetCartAsync(userId, It.IsAny<CancellationToken>(), It.IsAny<bool>()))
                         .ReturnsAsync((Cart?)null);

            var handler = CreateHandler();

            await Assert.ThrowsAsync<CartNotFoundException>(() => handler.Handle(command, default));
        }
    }
}

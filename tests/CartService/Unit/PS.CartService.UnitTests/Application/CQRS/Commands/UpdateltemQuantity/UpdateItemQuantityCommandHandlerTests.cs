using MediatR;
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
            // Arrange
            var userId = Guid.NewGuid();
            var bookId = Guid.NewGuid();
            var command = new UpdateItemQuantityCommand(userId, bookId, 5);

            _cartRepoMock.Setup(r => r.GetCartAsync(userId, It.IsAny<CancellationToken>(), It.IsAny<bool>()))
                         .ReturnsAsync((Cart?)null);

            var handler = CreateHandler();

            // Act + Assert
            await Assert.ThrowsAsync<CartNotFoundException>(() => handler.Handle(command, default));
        }

        [Fact]
        public async Task Handle_ItemNotFound_ShouldThrow()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var bookId = Guid.NewGuid();
            var cart = new Cart(userId); // пустая корзина

            var command = new UpdateItemQuantityCommand(userId, bookId, 5);

            _cartRepoMock.Setup(r => r.GetCartAsync(userId, It.IsAny<CancellationToken>(), It.IsAny<bool>()))
                         .ReturnsAsync(cart);

            var handler = CreateHandler();

            // Act + Assert
            await Assert.ThrowsAsync<CartItemNotFoundException>(() => handler.Handle(command, default));
        }

        [Fact]
        public async Task Handle_InvalidQuantity_ShouldThrow()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var bookId = Guid.NewGuid();
            var cart = new Cart(userId);
            cart.AddItem(bookId, 2, 50m); // товар в корзине

            var command = new UpdateItemQuantityCommand(userId, bookId, 0); // некорректное количество

            _cartRepoMock.Setup(r => r.GetCartAsync(userId, It.IsAny<CancellationToken>(), It.IsAny<bool>()))
                         .ReturnsAsync(cart);

            var handler = CreateHandler();

            // Act + Assert
            await Assert.ThrowsAsync<InvalidCartItemQuantityException>(() => handler.Handle(command, default));
        }

        [Fact]
        public async Task Handle_ValidUpdate_ShouldUpdateQuantityAndSave()
        {
            var userId = Guid.NewGuid();
            var bookId = Guid.NewGuid();
            var cart = new Cart(userId);
            cart.AddItem(bookId, 2, 50m);

            var command = new UpdateItemQuantityCommand(userId, bookId, 5);

            _cartRepoMock.Setup(r => r.GetCartAsync(userId, It.IsAny<CancellationToken>(), It.IsAny<bool>()))
                         .ReturnsAsync(cart);

            var handler = CreateHandler();

            var result = await handler.Handle(command, default);

            Assert.Equal(Unit.Value, result);
            Assert.Equal(5, cart.Items.First().Quantity);
            _cartRepoMock.Verify(r => r.UpdateAsync(cart, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}

using Microsoft.Extensions.Logging;
using Moq;
using PS.CartService.Application.Clients;
using PS.CartService.Application.CQRS.Commands.Checkout;
using PS.CartService.Application.DTOs.Order;
using PS.CartService.Application.Interfaces;
using PS.CartService.Domain.Entities;
using PS.CartService.Domain.Exceptions.Cart;

namespace PS.CartService.UnitTests.Application.CQRS.Commands.Checkout
{
    public class CheckoutCommandHandlerTests
    {
        private readonly Mock<ICartRepository> _cartRepoMock = new();
        private readonly Mock<IOrderServiceClient> _orderServiceClientMock = new();
        private readonly Mock<ILogger<CheckoutCommandHandler>> _loggerMock = new();

        private CheckoutCommandHandler CreateHandler() =>
            new(_cartRepoMock.Object, _orderServiceClientMock.Object, _loggerMock.Object);

        [Fact]
        public async Task Handle_CartNotFound_ShouldThrow()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var command = new CheckoutCommand(userId, "Some Address");

            _cartRepoMock.Setup(r => r.GetCartAsync(userId, It.IsAny<CancellationToken>(), It.IsAny<bool>()))
                         .ReturnsAsync((Cart?)null);

            var handler = CreateHandler();

            // Act + Assert
            await Assert.ThrowsAsync<CartNotFoundException>(() => handler.Handle(command, default));
        }

        [Fact]
        public async Task Handle_EmptyCart_ShouldThrow()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var emptyCart = new Cart(userId); // корзина без товаров
            var command = new CheckoutCommand(userId, "Some Address");

            _cartRepoMock.Setup(r => r.GetCartAsync(userId, It.IsAny<CancellationToken>(), It.IsAny<bool>()))
                         .ReturnsAsync(emptyCart);

            var handler = CreateHandler();

            // Act + Assert
            await Assert.ThrowsAsync<EmptyCartException>(() => handler.Handle(command, default));
        }

        [Fact]
        public async Task Handle_ValidCart_ShouldCreateOrder_AndClearCart()
        {
            var userId = Guid.NewGuid();
            var cart = new Cart(userId);
            cart.AddItem(Guid.NewGuid(), 2, 15m);

            var command = new CheckoutCommand(userId, "Test Street 123");

            var expectedOrder = new OrderResponse
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                TotalPrice = cart.TotalPrice,
                CreatedAt = DateTime.UtcNow
            };

            _cartRepoMock.Setup(r => r.GetCartAsync(userId, It.IsAny<CancellationToken>(), It.IsAny<bool>()))
                         .ReturnsAsync(cart);

            _orderServiceClientMock.Setup(c => c.CreateOrderAsync(It.IsAny<CreateOrderRequest>(), It.IsAny<CancellationToken>()))
                                   .ReturnsAsync(expectedOrder);

            var handler = CreateHandler();

            var result = await handler.Handle(command, default);

            Assert.Equal(expectedOrder.Id, result.Id);
            Assert.Equal(expectedOrder.TotalPrice, result.TotalPrice);
            Assert.Equal(expectedOrder.UserId, result.UserId);

            _cartRepoMock.Verify(r => r.UpdateAsync(cart, It.IsAny<CancellationToken>()), Times.Once);
            Assert.Empty(cart.Items); // корзина должна очиститься
        }
    }
}

using Microsoft.Extensions.Logging;
using Moq;
using PS.CartService.Application.CQRS.Queries.GetCart;
using PS.CartService.Application.Interfaces;
using PS.CartService.Domain.Entities;

namespace PS.CartService.UnitTests.Application.CQRS.Queries.GetCart
{
    public class GetCartQueryHandlerTests
    {
        private readonly Mock<ICartRepository> _cartRepoMock = new();
        private readonly Mock<ILogger<GetCartQueryHandler>> _loggerMock = new();

        private GetCartQueryHandler CreateHandler() =>
            new(_cartRepoMock.Object, _loggerMock.Object);

        [Fact]
        public async Task Handle_CartExists_ShouldReturnMappedResponse()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var cart = new Cart(userId);
            cart.AddItem(Guid.NewGuid(), 2, 100m);

            _cartRepoMock.Setup(r => r.GetCartAsync(userId, It.IsAny<CancellationToken>(), true))
                         .ReturnsAsync(cart);

            var query = new GetCartQuery(userId);
            var handler = CreateHandler();

            // Act
            var result = await handler.Handle(query, default);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userId, result.UserId);
            Assert.Single(result.Items);
            Assert.Equal(cart.Items.First().BookId, result.Items.First().BookId);
            Assert.Equal(cart.Items.First().Quantity, result.Items.First().Quantity);
            Assert.Equal(cart.Items.First().UnitPrice, result.Items.First().UnitPrice);
        }

    }
}

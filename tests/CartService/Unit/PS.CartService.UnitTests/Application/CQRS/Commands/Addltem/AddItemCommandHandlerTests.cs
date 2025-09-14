using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using PS.CartService.Application.Clients;
using PS.CartService.Application.CQRS.Commands.AddItem;
using PS.CartService.Application.Interfaces;
using PS.CartService.Domain.Entities;

namespace PS.CartService.UnitTests.Application.CQRS.Commands.Addltem
{
    public class AddItemCommandHandlerTests
    {
        private readonly Mock<ICartRepository> _cartRepoMock = new();
        private readonly Mock<IBookCatalogClient> _bookCatalogClientMock = new();
        private readonly Mock<ILogger<AddItemCommandHandler>> _loggerMock = new();

        private AddItemCommandHandler CreateHandler() =>
            new(_cartRepoMock.Object, _bookCatalogClientMock.Object, _loggerMock.Object);


        [Fact]
        public async Task Handle_NoExistingCart_ShouldCreateNewCart()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var bookId = Guid.NewGuid();
            var command = new AddItemCommand(userId, bookId, 2);

            _cartRepoMock.Setup(r => r.GetCartAsync(userId, It.IsAny<CancellationToken>(), It.IsAny<bool>()))
                         .ReturnsAsync((Cart?)null);

            _bookCatalogClientMock.Setup(c => c.GetBookPriceAsync(bookId, It.IsAny<CancellationToken>()))
                                  .ReturnsAsync(10m);

            var handler = CreateHandler();

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            Assert.Equal(Unit.Value, result);
            _cartRepoMock.Verify(r => r.AddAsync(It.IsAny<Cart>(), It.IsAny<CancellationToken>()), Times.Once);
            _cartRepoMock.Verify(r => r.UpdateAsync(It.IsAny<Cart>(), It.IsAny<CancellationToken>()), Times.Never);
        }


        [Fact]
        public async Task Handle_ExistingCart_ShouldUpdateCart()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var bookId = Guid.NewGuid();
            var existingCart = new Cart(userId);
            var command = new AddItemCommand(userId, bookId, 1);

            _cartRepoMock.Setup(r => r.GetCartAsync(userId, It.IsAny<CancellationToken>(), It.IsAny<bool>()))
                         .ReturnsAsync(existingCart);

            _bookCatalogClientMock.Setup(c => c.GetBookPriceAsync(bookId, It.IsAny<CancellationToken>()))
                                  .ReturnsAsync(20m);

            var handler = CreateHandler();

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            Assert.Equal(Unit.Value, result);
            _cartRepoMock.Verify(r => r.UpdateAsync(existingCart, It.IsAny<CancellationToken>()), Times.Once);
            _cartRepoMock.Verify(r => r.AddAsync(It.IsAny<Cart>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}

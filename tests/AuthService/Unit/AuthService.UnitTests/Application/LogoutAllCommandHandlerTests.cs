using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using PS.FreeBookHub_Lite.AuthService.Application.CQRS.Commands.LogoutAll;
using PS.FreeBookHub_Lite.AuthService.Application.Interfaces;

namespace AuthService.UnitTests.Application
{
    public class LogoutAllCommandHandlerTests
    {
        private readonly Mock<IRefreshTokenRepository> _refreshTokenRepoMock = new();
        private readonly Mock<ILogger<LogoutAllCommandHandler>> _loggerMock = new();

        private LogoutAllCommandHandler CreateHandler() =>
            new(_refreshTokenRepoMock.Object, _loggerMock.Object);

        [Fact]
        public async Task Handle_ValidRequest_ShouldRevokeAllTokens()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var command = new LogoutAllCommand { UserId = userId };

            _refreshTokenRepoMock.Setup(x =>
                x.RevokeAllTokensForUserAsync(userId, It.IsAny<CancellationToken>())
            ).Returns(Task.CompletedTask);

            var handler = CreateHandler();

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal(Unit.Value, result);

            _refreshTokenRepoMock.Verify(x =>
                x.RevokeAllTokensForUserAsync(userId, It.IsAny<CancellationToken>()),
                Times.Once
            );
        }

        [Fact]
        public async Task Handle_NullRequest_ShouldThrowArgumentNullException()
        {
            var handler = CreateHandler();

            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                handler.Handle(null!, CancellationToken.None));
        }
    }
}

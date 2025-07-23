using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using PS.FreeBookHub_Lite.AuthService.Application.CQRS.Commands.Logout;
using PS.FreeBookHub_Lite.AuthService.Application.Interfaces;
using PS.FreeBookHub_Lite.AuthService.Domain.Entities;
using PS.FreeBookHub_Lite.AuthService.Domain.Exceptions.Token;

namespace AuthService.UnitTests.Application.Commands.Logout
{
    public class LogoutCommandHandlerTests
    {
        private readonly Mock<IRefreshTokenRepository> _refreshTokenRepoMock = new();
        private readonly Mock<ILogger<LogoutCommandHandler>> _loggerMock = new();

        private LogoutCommandHandler CreateHandler() =>
            new(_refreshTokenRepoMock.Object, _loggerMock.Object);

        [Fact]
        public async Task Handle_TokenNotFound_ShouldThrowTokenNotFoundException()
        {
            // Arrange
            _refreshTokenRepoMock.Setup(x =>
                x.GetByTokenAsync(It.IsAny<string>(), It.IsAny<CancellationToken>(), false)
            ).ReturnsAsync((RefreshToken?)null);

            var handler = CreateHandler();
            var command = new LogoutCommand { RefreshToken = "invalid-token" };

            // Act & Assert
            await Assert.ThrowsAsync<TokenNotFoundException>(() =>
                handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_TokenRevoked_ShouldThrowRevokedTokenException()
        {
            var revokedToken = CreateInactiveToken();
            revokedToken.Revoke(); // вручную ревокаем

            _refreshTokenRepoMock.Setup(x =>
                x.GetByTokenAsync(It.IsAny<string>(), It.IsAny<CancellationToken>(), false)
            ).ReturnsAsync(revokedToken);

            var handler = CreateHandler();
            var command = new LogoutCommand { RefreshToken = "revoked-token" };

            await Assert.ThrowsAsync<RevokedTokenException>(() =>
                handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ValidToken_ShouldRevokeAndUpdate()
        {
            var token = new RefreshToken(Guid.NewGuid(), "token123", DateTime.UtcNow.AddMinutes(10));

            _refreshTokenRepoMock.Setup(x =>
                x.GetByTokenAsync("token123", It.IsAny<CancellationToken>(), false)
            ).ReturnsAsync(token);

            _refreshTokenRepoMock.Setup(x =>
                x.UpdateAsync(token, It.IsAny<CancellationToken>())
            ).Returns(Task.CompletedTask);

            var handler = CreateHandler();
            var command = new LogoutCommand { RefreshToken = "token123" };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal(Unit.Value, result);
            Assert.True(token.IsRevoked);
        }

        [Fact]
        public async Task Handle_NullRequest_ShouldThrowArgumentNullException()
        {
            var handler = CreateHandler();
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                handler.Handle(null!, CancellationToken.None));
        }

        private RefreshToken CreateInactiveToken()
        {
            var token = new RefreshToken(Guid.NewGuid(), "token123", DateTime.UtcNow.AddMinutes(-10)); // expired
            return token;
        }
    }
}

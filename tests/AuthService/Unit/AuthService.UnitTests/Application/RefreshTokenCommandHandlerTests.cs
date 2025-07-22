using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using PS.FreeBookHub_Lite.AuthService.Application.CQRS.Commands.RefreshToken;
using PS.FreeBookHub_Lite.AuthService.Application.Interfaces;
using PS.FreeBookHub_Lite.AuthService.Application.Services.Interfaces;
using PS.FreeBookHub_Lite.AuthService.Domain.Entities;
using PS.FreeBookHub_Lite.AuthService.Domain.Enums;
using PS.FreeBookHub_Lite.AuthService.Domain.Exceptions.Token;
using PS.FreeBookHub_Lite.AuthService.Domain.Exceptions.User;


namespace AuthService.UnitTests.Application
{
    public class RefreshTokenCommandHandlerTests
    {
        private readonly Mock<IRefreshTokenRepository> _refreshTokenRepoMock = new();
        private readonly Mock<IUserRepository> _userRepoMock = new();
        private readonly Mock<ITokenService> _tokenServiceMock = new();
        private readonly Mock<IConfiguration> _configMock = new();
        private readonly Mock<ILogger<RefreshTokenCommandHandler>> _loggerMock = new();

        private readonly RefreshTokenCommandHandler _handler;

        public RefreshTokenCommandHandlerTests()
        {
            _configMock.Setup(c => c["Auth:JwtSettings:RefreshTokenExpiryDays"]).Returns("7");
            _configMock.Setup(c => c["Auth:JwtSettings:AccessTokenExpiryMinutes"]).Returns("15");

            _handler = new RefreshTokenCommandHandler(
                _refreshTokenRepoMock.Object,
                _userRepoMock.Object,
                _tokenServiceMock.Object,
            _configMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task Handle_ValidTokenAndUser_ShouldReturnNewAuthResponse()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var oldToken = new RefreshToken(userId, "oldToken", DateTime.UtcNow.AddDays(1));
            var command = new RefreshTokenCommand { RefreshToken = "oldToken" };

            var user = new User("test@mail.com", "hashed", UserRole.User);

            _refreshTokenRepoMock.Setup(r => r.GetByTokenAsync("oldToken", It.IsAny<CancellationToken>(), false))
                .ReturnsAsync(oldToken);
            _userRepoMock.Setup(u => u.GetByIdAsync(userId, It.IsAny<CancellationToken>(), true))
                .ReturnsAsync(user);
            _tokenServiceMock.Setup(t => t.GenerateAccessToken(user)).Returns("newAccessToken");
            _tokenServiceMock.Setup(t => t.GenerateRefreshToken()).Returns("newRefreshToken");

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("newAccessToken", result.AccessToken);
            Assert.Equal("newRefreshToken", result.RefreshToken);

            _refreshTokenRepoMock.Verify(r => r.UpdateAsync(It.Is<RefreshToken>(t => !t.IsActive()), It.IsAny<CancellationToken>()), Times.Once);
            _refreshTokenRepoMock.Verify(r => r.AddAsync(It.Is<RefreshToken>(t => t.Token == "newRefreshToken"), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_TokenNotFound_ShouldThrowInvalidTokenException()
        {
            // Arrange
            var command = new RefreshTokenCommand { RefreshToken = "missing" };
            _refreshTokenRepoMock.Setup(r => r.GetByTokenAsync("missing", It.IsAny<CancellationToken>(), false))
                .ReturnsAsync((RefreshToken?)null);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidTokenException>(() =>
                _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_TokenRevoked_ShouldThrowInvalidTokenException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var revokedToken = new RefreshToken(userId, "token", DateTime.UtcNow.AddDays(-1));
            revokedToken.Revoke();

            var command = new RefreshTokenCommand { RefreshToken = "token" };

            _refreshTokenRepoMock.Setup(r => r.GetByTokenAsync("token", It.IsAny<CancellationToken>(), false))
                .ReturnsAsync(revokedToken);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidTokenException>(() =>
                _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_UserNotFound_ShouldThrowUserByIdNotFoundException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var token = new RefreshToken(userId, "token", DateTime.UtcNow.AddDays(1));

            var command = new RefreshTokenCommand { RefreshToken = "token" };

            _refreshTokenRepoMock.Setup(r => r.GetByTokenAsync("token", It.IsAny<CancellationToken>(), false))
                .ReturnsAsync(token);

            _userRepoMock.Setup(u => u.GetByIdAsync(userId, It.IsAny<CancellationToken>(), true))
                .ReturnsAsync((User?)null);

            // Act & Assert
            await Assert.ThrowsAsync<UserByIdNotFoundException>(() =>
                _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_UserNotActive_ShouldThrowUserByIdNotFoundException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var token = new RefreshToken(userId, "token", DateTime.UtcNow.AddDays(1));
            var user = new User("test@mail.com", "hash", UserRole.User); // по умолчанию не активен
            user.Deactivate();

            var command = new RefreshTokenCommand { RefreshToken = "token" };

            _refreshTokenRepoMock.Setup(r => r.GetByTokenAsync("token", It.IsAny<CancellationToken>(), false))
                .ReturnsAsync(token);

            _userRepoMock.Setup(u => u.GetByIdAsync(userId, It.IsAny<CancellationToken>(), true))
                .ReturnsAsync(user);

            // Act & Assert
            await Assert.ThrowsAsync<UserByIdNotFoundException>(() =>
                _handler.Handle(command, CancellationToken.None));
        }
    }
}

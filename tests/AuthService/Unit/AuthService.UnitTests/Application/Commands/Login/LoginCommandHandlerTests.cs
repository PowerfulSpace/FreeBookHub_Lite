using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using PS.FreeBookHub_Lite.AuthService.Application.CQRS.Commands.Login;
using PS.FreeBookHub_Lite.AuthService.Application.Interfaces;
using PS.FreeBookHub_Lite.AuthService.Application.Services.Interfaces;
using PS.FreeBookHub_Lite.AuthService.Domain.Entities;
using PS.FreeBookHub_Lite.AuthService.Domain.Exceptions.User;

namespace AuthService.UnitTests.Application.Commands.Login
{
    public class LoginCommandHandlerTests
    {
        private readonly Mock<IUserRepository> _userRepoMock = new();
        private readonly Mock<ITokenService> _tokenServiceMock = new();
        private readonly Mock<IConfiguration> _configMock = new();
        private readonly Mock<IRefreshTokenRepository> _refreshRepoMock = new();
        private readonly Mock<IPasswordHasher> _passwordHasherMock = new();
        private readonly Mock<ILogger<LoginCommandHandler>> _loggerMock = new();

        private LoginCommandHandler CreateHandler() =>
            new(_userRepoMock.Object, _tokenServiceMock.Object, _configMock.Object,
                _refreshRepoMock.Object, _passwordHasherMock.Object, _loggerMock.Object);

        [Fact]
        public async Task Handle_UserNotFound_ShouldThrowInvalidCredentialsException()
        {
            // Arrange
            _userRepoMock.Setup(x => x.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>(), true))
                         .ReturnsAsync((User?)null);

            var handler = CreateHandler();
            var command = new LoginCommand { Email = "not@found.com", Password = "password" };

            // Act + Assert
            await Assert.ThrowsAsync<InvalidCredentialsException>(() => handler.Handle(command, default));
        }

        [Fact]
        public async Task Handle_UserDeactivated_ShouldThrowDeactivatedUserException()
        {
            var user = new User("email", "hash");
            user.Deactivate();

            _userRepoMock.Setup(x => x.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>(), true))
                         .ReturnsAsync(user);

            var handler = CreateHandler();
            var command = new LoginCommand { Email = "email", Password = "password" };

            await Assert.ThrowsAsync<DeactivatedUserException>(() => handler.Handle(command, default));
        }

        [Fact]
        public async Task Handle_InvalidPassword_ShouldThrowInvalidCredentialsException()
        {
            var user = new User("email", "hash");

            _userRepoMock.Setup(x => x.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>(), true))
                         .ReturnsAsync(user);

            _passwordHasherMock.Setup(x => x.Verify("password", user.PasswordHash))
                               .Returns(false);

            var handler = CreateHandler();
            var command = new LoginCommand { Email = "email", Password = "password" };

            await Assert.ThrowsAsync<InvalidCredentialsException>(() => handler.Handle(command, default));
        }

        [Fact]
        public async Task Handle_ValidCredentials_ShouldReturnAuthResponse()
        {
            var user = new User("email", "hashed");
            var accessToken = "access-token";
            var refreshToken = "refresh-token";

            _userRepoMock.Setup(x => x.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>(), true))
                         .ReturnsAsync(user);

            _passwordHasherMock.Setup(x => x.Verify("password", user.PasswordHash))
                               .Returns(true);

            _tokenServiceMock.Setup(x => x.GenerateAccessToken(user)).Returns(accessToken);
            _tokenServiceMock.Setup(x => x.GenerateRefreshToken()).Returns(refreshToken);

            _configMock.Setup(x => x["Auth:JwtSettings:RefreshTokenExpiryDays"]).Returns("7");
            _configMock.Setup(x => x["Auth:JwtSettings:AccessTokenExpiryMinutes"]).Returns("15");

            _refreshRepoMock.Setup(x => x.AddAsync(It.IsAny<RefreshToken>(), It.IsAny<CancellationToken>()))
                            .Returns(Task.CompletedTask);

            var handler = CreateHandler();
            var command = new LoginCommand { Email = "email", Password = "password" };

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            Assert.Equal(accessToken, result.AccessToken);
            Assert.Equal(refreshToken, result.RefreshToken);
            Assert.True(result.ExpiresAt > DateTime.UtcNow);
        }
    }
}

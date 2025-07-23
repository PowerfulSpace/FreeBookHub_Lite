using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using PS.FreeBookHub_Lite.AuthService.Application.CQRS.Commands.Register;
using PS.FreeBookHub_Lite.AuthService.Application.Interfaces;
using PS.FreeBookHub_Lite.AuthService.Application.Services.Interfaces;
using PS.FreeBookHub_Lite.AuthService.Domain.Entities;
using PS.FreeBookHub_Lite.AuthService.Domain.Exceptions.User;

namespace AuthService.UnitTests.Application.Commands.Register
{
    public class RegisterCommandHandlerTests
    {
        private readonly Mock<IUserRepository> _userRepoMock = new();
        private readonly Mock<ITokenService> _tokenServiceMock = new();
        private readonly Mock<IConfiguration> _configMock = new();
        private readonly Mock<IRefreshTokenRepository> _refreshRepoMock = new();
        private readonly Mock<IPasswordHasher> _passwordHasherMock = new();
        private readonly Mock<ILogger<RegisterCommandHandler>> _loggerMock = new();

        private RegisterCommandHandler CreateHandler()
        {
            return new RegisterCommandHandler(
                _userRepoMock.Object,
                _tokenServiceMock.Object,
                _configMock.Object,
                _refreshRepoMock.Object,
                _passwordHasherMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task Handle_UserAlreadyExists_ShouldThrowUserAlreadyExistsException()
        {
            // Arrange
            var existingUser = new User("existing@mail.com", "hash");
            _userRepoMock
                .Setup(x => x.GetByEmailAsync(existingUser.Email, It.IsAny<CancellationToken>(), true))
                .ReturnsAsync(existingUser);

            var handler = CreateHandler();
            var command = new RegisterCommand
            {
                Email = existingUser.Email,
                Password = "password",
                Role = "User"
            };

            // Act & Assert
            await Assert.ThrowsAsync<UserAlreadyExistsException>(() => handler.Handle(command, default));
        }

        [Fact]
        public async Task Handle_ValidData_ShouldReturnAuthResponse()
        {
            // Arrange
            var email = "newuser@mail.com";
            var password = "secure";
            var hashedPassword = "hashed_secure";
            var accessToken = "access-token";
            var refreshToken = "refresh-token";

            _userRepoMock
                .Setup(x => x.GetByEmailAsync(email, It.IsAny<CancellationToken>(), true))
                .ReturnsAsync((User?)null);


            _passwordHasherMock
                .Setup(x => x.Hash(password))
                .Returns(hashedPassword);

            _tokenServiceMock
                .Setup(x => x.GenerateAccessToken(It.IsAny<User>()))
                .Returns(accessToken);

            _tokenServiceMock
                .Setup(x => x.GenerateRefreshToken())
                .Returns(refreshToken);

            _configMock
                .Setup(x => x["Auth:JwtSettings:RefreshTokenExpiryDays"])
                .Returns("7");

            _configMock
                .Setup(x => x["Auth:JwtSettings:AccessTokenExpiryMinutes"])
                .Returns("15");

            _userRepoMock
                .Setup(x => x.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);


            _refreshRepoMock
                .Setup(x => x.AddAsync(It.IsAny<RefreshToken>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var handler = CreateHandler();

            var command = new RegisterCommand
            {
                Email = email,
                Password = password,
                Role = "User"
            };

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            Assert.Equal(accessToken, result.AccessToken);
            Assert.Equal(refreshToken, result.RefreshToken);
            Assert.True(result.ExpiresAt > DateTime.UtcNow);

            _userRepoMock.Verify(x => x.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
            _refreshRepoMock.Verify(x => x.AddAsync(It.IsAny<RefreshToken>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}

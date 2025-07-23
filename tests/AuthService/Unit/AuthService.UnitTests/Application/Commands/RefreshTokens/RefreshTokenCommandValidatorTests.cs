using FluentValidation.TestHelper;
using Moq;
using PS.FreeBookHub_Lite.AuthService.Application.CQRS.Commands.RefreshToken;
using PS.FreeBookHub_Lite.AuthService.Application.Interfaces;
using PS.FreeBookHub_Lite.AuthService.Domain.Entities;

namespace AuthService.UnitTests.Application.Commands.RefreshTokens
{
    public class RefreshTokenCommandValidatorTests
    {
        private readonly Mock<IRefreshTokenRepository> _refreshTokenRepoMock = new();
        private readonly Mock<IUserRepository> _userRepoMock = new();
        private readonly RefreshTokenCommandValidator _validator;

        public RefreshTokenCommandValidatorTests()
        {
            _validator = new RefreshTokenCommandValidator(
                _refreshTokenRepoMock.Object,
                _userRepoMock.Object
            );
        }

        [Fact]
        public async Task Should_HaveValidationError_When_TokenIsNullOrExpired()
        {
            // Arrange: токен не найден
            _refreshTokenRepoMock
                .Setup(x => x.GetByTokenAsync(It.IsAny<string>(), It.IsAny<CancellationToken>(), true))
                .ReturnsAsync((RefreshToken?)null);

            var model = new RefreshTokenCommand { RefreshToken = "invalid-token" };

            // Act
            var result = await _validator.TestValidateAsync(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.RefreshToken)
                  .WithErrorMessage("Invalid or expired refresh token.");
        }

        [Fact]
        public async Task Should_HaveValidationError_When_TokenIsRevoked()
        {
            var token = new RefreshToken(Guid.NewGuid(), "revoked-token", DateTime.UtcNow.AddDays(1));
            token.Revoke(); // делаем токен неактивным

            _refreshTokenRepoMock
                .Setup(x => x.GetByTokenAsync(It.IsAny<string>(), It.IsAny<CancellationToken>(), true))
                .ReturnsAsync(token);

            var model = new RefreshTokenCommand { RefreshToken = "revoked-token" };

            var result = await _validator.TestValidateAsync(model);

            result.ShouldHaveValidationErrorFor(x => x.RefreshToken)
                  .WithErrorMessage("Invalid or expired refresh token.");
        }

        [Fact]
        public async Task Should_HaveValidationError_When_UserIsDeactivated()
        {
            // Arrange: активный токен
            var userId = Guid.NewGuid();
            var token = new RefreshToken(userId, "valid-token", DateTime.UtcNow.AddDays(1));
            _refreshTokenRepoMock
                .Setup(x => x.GetByTokenAsync(It.IsAny<string>(), It.IsAny<CancellationToken>(), true))
                .ReturnsAsync(token);

            // Но пользователь — деактивирован
            var user = new User("email@test.com", "hash");
            user.Deactivate();

            _userRepoMock
                .Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>(), true))
                .ReturnsAsync(user);

            var model = new RefreshTokenCommand { RefreshToken = "valid-token" };

            var result = await _validator.TestValidateAsync(model);

            result.ShouldHaveValidationErrorFor(x => x.RefreshToken)
                  .WithErrorMessage("Associated user account is not active.");
        }

        [Fact]
        public async Task Should_NotHaveValidationError_When_TokenAndUserAreValid()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var token = new RefreshToken(userId, "good-token", DateTime.UtcNow.AddDays(1));

            var user = new User("user@valid.com", "hashedPassword"); // активный по умолчанию

            _refreshTokenRepoMock
                .Setup(x => x.GetByTokenAsync(It.IsAny<string>(), It.IsAny<CancellationToken>(), true))
                .ReturnsAsync(token);

            _userRepoMock
                .Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>(), true))
                .ReturnsAsync(user);

            var model = new RefreshTokenCommand { RefreshToken = "good-token" };

            var result = await _validator.TestValidateAsync(model);

            result.ShouldNotHaveValidationErrorFor(x => x.RefreshToken);
        }
    }
}

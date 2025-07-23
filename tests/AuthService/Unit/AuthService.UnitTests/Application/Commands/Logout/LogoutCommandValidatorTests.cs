using FluentValidation.TestHelper;
using Moq;
using PS.FreeBookHub_Lite.AuthService.Application.CQRS.Commands.Logout;
using PS.FreeBookHub_Lite.AuthService.Application.Interfaces;
using PS.FreeBookHub_Lite.AuthService.Domain.Entities;

namespace AuthService.UnitTests.Application.Commands.Logout
{
    public class LogoutCommandValidatorTests
    {
        private readonly Mock<IRefreshTokenRepository> _refreshTokenRepoMock = new();
        private readonly LogoutCommandValidator _validator;

        public LogoutCommandValidatorTests()
        {
            _validator = new LogoutCommandValidator(_refreshTokenRepoMock.Object);
        }

        [Fact]
        public async Task Should_HaveValidationError_When_TokenNotFound()
        {
            // Arrange
            _refreshTokenRepoMock
                .Setup(x => x.GetByTokenAsync(It.IsAny<string>(), It.IsAny<CancellationToken>(), true))
                .ReturnsAsync((RefreshToken?)null);

            var model = new LogoutCommand { RefreshToken = "invalid-token" };

            // Act
            var result = await _validator.TestValidateAsync(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.RefreshToken)
                  .WithErrorMessage("Refresh token not found.");
        }

        [Fact]
        public async Task Should_HaveValidationError_When_TokenRevoked()
        {
            // Arrange
            var token = new RefreshToken(Guid.NewGuid(), "token-value", DateTime.UtcNow.AddDays(1));
            token.Revoke(); // делает токен неактивным

            _refreshTokenRepoMock
                .Setup(x => x.GetByTokenAsync(It.IsAny<string>(), It.IsAny<CancellationToken>(), true))
                .ReturnsAsync(token);

            var model = new LogoutCommand { RefreshToken = "token-value" };

            // Act
            var result = await _validator.TestValidateAsync(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.RefreshToken)
                  .WithErrorMessage("Refresh token is already revoked.");
        }

        [Fact]
        public async Task Should_NotHaveValidationError_When_TokenIsActive()
        {
            // Arrange
            var token = new RefreshToken(Guid.NewGuid(), "valid-token", DateTime.UtcNow.AddDays(1));
            // активный по умолчанию

            _refreshTokenRepoMock
                .Setup(x => x.GetByTokenAsync(It.IsAny<string>(), It.IsAny<CancellationToken>(), true))
                .ReturnsAsync(token);

            var model = new LogoutCommand { RefreshToken = "valid-token" };

            // Act
            var result = await _validator.TestValidateAsync(model);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.RefreshToken);
        }
    }
}

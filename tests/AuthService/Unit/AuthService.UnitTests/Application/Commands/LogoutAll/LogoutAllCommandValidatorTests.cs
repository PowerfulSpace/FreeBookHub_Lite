using FluentValidation.TestHelper;
using PS.AuthService.Application.CQRS.Commands.LogoutAll;

namespace AuthService.UnitTests.Application.Commands.LogoutAll
{
    public class LogoutAllCommandValidatorTests
    {
        private readonly LogoutAllCommandValidator _validator = new();

        [Fact]
        public async Task Should_HaveValidationError_When_UserIdIsEmpty()
        {
            // Arrange
            var model = new LogoutAllCommand { UserId = Guid.Empty };

            // Act
            var result = await _validator.TestValidateAsync(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.UserId)
                  .WithErrorMessage("UserId is required.");
        }

        [Fact]
        public async Task Should_NotHaveValidationError_When_UserIdIsValid()
        {
            // Arrange
            var model = new LogoutAllCommand { UserId = Guid.NewGuid() };

            // Act
            var result = await _validator.TestValidateAsync(model);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.UserId);
        }
    }
}

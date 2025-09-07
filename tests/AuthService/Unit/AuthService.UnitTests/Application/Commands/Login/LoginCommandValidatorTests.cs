using FluentValidation.TestHelper;
using Moq;
using PS.AuthService.Application.CQRS.Commands.Login;
using PS.AuthService.Application.Interfaces;
using PS.AuthService.Domain.Entities;

namespace AuthService.UnitTests.Application.Commands.Login
{
    public class LoginCommandValidatorTests
    {
        private readonly Mock<IUserRepository> _userRepoMock = new();
        private readonly LoginCommandValidator _validator;

        public LoginCommandValidatorTests()
        {
            _validator = new LoginCommandValidator(_userRepoMock.Object);
        }

        [Fact]
        public async Task Should_HaveValidationError_When_UserDoesNotExist()
        {
            // Arrange
            _userRepoMock.Setup(x => x.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>(), true))
                         .ReturnsAsync((User?)null);

            var model = new LoginCommand { Email = "not@found.com", Password = "irrelevant" };

            // Act
            var result = await _validator.TestValidateAsync(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Email)
                  .WithErrorMessage("Invalid credentials.");
        }

        [Fact]
        public async Task Should_HaveValidationError_When_UserIsDeactivated()
        {
            // Arrange
            var user = new User("deactivated@user.com", "hashedpwd");
            user.Deactivate();

            _userRepoMock.Setup(x => x.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>(), true))
                         .ReturnsAsync(user);

            var model = new LoginCommand { Email = "deactivated@user.com", Password = "irrelevant" };

            // Act
            var result = await _validator.TestValidateAsync(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Email)
                  .WithErrorMessage("This user account has been deactivated.");
        }

        [Fact]
        public async Task Should_NotHaveValidationError_When_UserExistsAndActive()
        {
            // Arrange
            var user = new User("active@user.com", "hashedpwd");

            _userRepoMock.Setup(x => x.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>(), true))
                         .ReturnsAsync(user);

            var model = new LoginCommand { Email = "active@user.com", Password = "irrelevant" };

            // Act
            var result = await _validator.TestValidateAsync(model);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.Email);
        }
    }
}

using FluentValidation.TestHelper;
using Moq;
using PS.AuthService.Application.CQRS.Commands.Register;
using PS.AuthService.Application.Interfaces;
using PS.AuthService.Domain.Entities;

namespace PS.AuthService.UnitTests.Application.Commands.Register
{
    public class RegisterCommandValidatorTests
    {
        private readonly Mock<IUserRepository> _userRepoMock = new();
        private readonly RegisterCommandValidator _validator;

        public RegisterCommandValidatorTests()
        {
            _validator = new RegisterCommandValidator(_userRepoMock.Object);
        }

        [Fact]
        public async Task Should_HaveValidationError_When_EmailAlreadyExists()
        {
            // Arrange: пользователь с таким email существует
            var existingUser = new User("test@example.com", "hashed-password");

            _userRepoMock
                .Setup(x => x.GetByEmailAsync("test@example.com", It.IsAny<CancellationToken>(), true))
                .ReturnsAsync(existingUser);

            var model = new RegisterCommand
            {
                Email = "test@example.com",
                Password = "AnyPassword"
            };

            // Act
            var result = await _validator.TestValidateAsync(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Email)
                  .WithErrorMessage("A user with this email already exists.");
        }

        [Fact]
        public async Task Should_NotHaveValidationError_When_EmailIsUnique()
        {
            // Arrange: пользователь не найден
            _userRepoMock
                .Setup(x => x.GetByEmailAsync("unique@example.com", It.IsAny<CancellationToken>(), true))
                .ReturnsAsync((User?)null);

            var model = new RegisterCommand
            {
                Email = "unique@example.com",
                Password = "SomePassword123"
            };

            // Act
            var result = await _validator.TestValidateAsync(model);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.Email);
        }
    }
}

using FluentValidation.TestHelper;
using PS.FreeBookHub_Lite.AuthService.Application.DTOs;
using PS.FreeBookHub_Lite.AuthService.Application.Validators;

namespace AuthService.UnitTests.Application.Validators
{
    public class RegisterUserRequestValidatorTests
    {
        private readonly RegisterUserRequestValidator _validator = new();

        [Fact]
        public void Should_Have_Error_When_Email_Is_Empty()
        {
            var model = new RegisterUserRequest { Email = "", Password = "validpass", Role = "User" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Email)
                  .WithErrorMessage("Email is required");
        }

        [Fact]
        public void Should_Have_Error_When_Email_Is_Invalid()
        {
            var model = new RegisterUserRequest { Email = "invalid-email", Password = "validpass", Role = "User" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Email)
                  .WithErrorMessage("Invalid email format");
        }

        [Fact]
        public void Should_Have_Error_When_Password_Is_Empty()
        {
            var model = new RegisterUserRequest { Email = "user@example.com", Password = "", Role = "User" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Password)
                  .WithErrorMessage("Password is required");
        }

        [Fact]
        public void Should_Have_Error_When_Password_Is_Too_Short()
        {
            var model = new RegisterUserRequest { Email = "user@example.com", Password = "123", Role = "User" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Password)
                  .WithErrorMessage("Password must be at least 6 characters long");
        }

        [Fact]
        public void Should_Have_Error_When_Role_Is_Invalid()
        {
            var model = new RegisterUserRequest { Email = "user@example.com", Password = "validpass", Role = "Admin" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Role)
                  .WithErrorMessage("Invalid role");
        }

        [Fact]
        public void Should_Not_Have_Error_When_Role_Is_Null_Or_Empty()
        {
            var model1 = new RegisterUserRequest { Email = "user@example.com", Password = "validpass", Role = null };
            var model2 = new RegisterUserRequest { Email = "user@example.com", Password = "validpass", Role = "" };

            var result1 = _validator.TestValidate(model1);
            var result2 = _validator.TestValidate(model2);

            result1.ShouldNotHaveValidationErrorFor(x => x.Role);
            result2.ShouldNotHaveValidationErrorFor(x => x.Role);
        }

        [Fact]
        public void Should_Not_Have_Errors_When_Request_Is_Valid()
        {
            var model = new RegisterUserRequest
            {
                Email = "user@example.com",
                Password = "securePassword",
                Role = "User"
            };

            var result = _validator.TestValidate(model);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}

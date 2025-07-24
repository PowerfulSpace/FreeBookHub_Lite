using FluentValidation.TestHelper;
using PS.FreeBookHub_Lite.AuthService.Application.DTOs;
using PS.FreeBookHub_Lite.AuthService.Application.Validators;

namespace AuthService.UnitTests.Application.Validators
{
    public class LoginRequestValidatorTests
    {
        private readonly LoginRequestValidator _validator = new();

        [Fact]
        public void Should_Have_Error_When_Email_Is_Empty()
        {
            var model = new LoginRequest { Email = "", Password = "ValidPassword123" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Email);
        }

        [Fact]
        public void Should_Have_Error_When_Email_Is_Invalid()
        {
            var model = new LoginRequest { Email = "not-an-email", Password = "ValidPassword123" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Email);
        }

        [Fact]
        public void Should_Have_Error_When_Password_Is_Empty()
        {
            var model = new LoginRequest { Email = "user@example.com", Password = "" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Password);
        }

        [Fact]
        public void Should_Not_Have_Errors_When_Fields_Are_Valid()
        {
            var model = new LoginRequest { Email = "user@example.com", Password = "ValidPassword123" };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}

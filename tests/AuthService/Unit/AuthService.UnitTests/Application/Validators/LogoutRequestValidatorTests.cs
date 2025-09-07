using FluentValidation.TestHelper;
using PS.AuthService.Application.DTOs;
using PS.AuthService.Application.Validators;

namespace AuthService.UnitTests.Application.Validators
{
    public class LogoutRequestValidatorTests
    {
        private readonly LogoutRequestValidator _validator = new();

        [Fact]
        public void Should_Have_Error_When_RefreshToken_Is_Empty()
        {
            var model = new LogoutRequest { RefreshToken = "" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.RefreshToken)
                  .WithErrorMessage("Refresh token is required");
        }

        [Fact]
        public void Should_Have_Error_When_RefreshToken_Is_Too_Short()
        {
            var model = new LogoutRequest { RefreshToken = "short-token" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.RefreshToken)
                  .WithErrorMessage("Invalid token format");
        }

        [Fact]
        public void Should_Not_Have_Errors_When_RefreshToken_Is_Valid()
        {
            var model = new LogoutRequest { RefreshToken = new string('a', 32) }; // 32 символа
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}

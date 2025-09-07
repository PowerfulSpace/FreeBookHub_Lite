using FluentValidation.TestHelper;
using PS.AuthService.Application.DTOs;
using PS.AuthService.Application.Validators;

namespace PS.AuthService.UnitTests.Application.Validators
{
    public class RefreshTokenRequestValidatorTests
    {
        private readonly RefreshTokenRequestValidator _validator = new();

        [Fact]
        public void Should_Have_Error_When_RefreshToken_Is_Empty()
        {
            var model = new RefreshTokenRequest { RefreshToken = "" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.RefreshToken)
                  .WithErrorMessage("Refresh token is required");
        }

        [Fact]
        public void Should_Not_Have_Errors_When_RefreshToken_Is_Valid()
        {
            var model = new RefreshTokenRequest { RefreshToken = "valid-refresh-token" };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}

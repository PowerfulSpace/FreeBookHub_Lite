using FluentValidation;
using PS.FreeBookHub_Lite.AuthService.Application.DTOs;

namespace PS.FreeBookHub_Lite.AuthService.Application.Validators
{
    public class RefreshTokenRequestValidator : AbstractValidator<RefreshTokenRequest>
    {
        public RefreshTokenRequestValidator()
        {
            RuleFor(x => x.RefreshToken)
                .NotEmpty().WithMessage("Refresh token is required");
        }
    }
}

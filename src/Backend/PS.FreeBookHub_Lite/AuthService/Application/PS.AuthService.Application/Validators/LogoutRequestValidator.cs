using FluentValidation;
using PS.AuthService.Application.DTOs;

namespace PS.AuthService.Application.Validators
{
    public class LogoutRequestValidator : AbstractValidator<LogoutRequest>
    {
        public LogoutRequestValidator()
        {
            RuleFor(x => x.RefreshToken)
                .NotEmpty().WithMessage("Refresh token is required")
                .MinimumLength(32).WithMessage("Invalid token format");
        }
    }
}

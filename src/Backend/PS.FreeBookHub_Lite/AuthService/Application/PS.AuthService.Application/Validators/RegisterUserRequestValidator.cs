using FluentValidation;
using PS.AuthService.Application.DTOs;

namespace PS.AuthService.Application.Validators
{
    public class RegisterUserRequestValidator : AbstractValidator<RegisterUserRequest>
    {
        public RegisterUserRequestValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters long");

            RuleFor(x => x.Role)
                .Must(role => string.IsNullOrEmpty(role) || role is "User")
                .WithMessage("Invalid role");
        }
    }
}

using FluentValidation;
using PS.AuthService.Application.Interfaces;

namespace PS.AuthService.Application.CQRS.Commands.Login
{
    public class LoginCommandValidator : AbstractValidator<LoginCommand>
    {
        public LoginCommandValidator(IUserRepository userRepository)
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .CustomAsync(async (email, context, ct) =>
                {
                    var user = await userRepository.GetByEmailAsync(email, ct, asNoTracking: true);

                    if (user == null)
                    {
                        context.AddFailure("Invalid credentials.");
                        return;
                    }

                    if (!user.IsActive)
                    {
                        context.AddFailure("This user account has been deactivated.");
                    }
                });
        }
    }
}

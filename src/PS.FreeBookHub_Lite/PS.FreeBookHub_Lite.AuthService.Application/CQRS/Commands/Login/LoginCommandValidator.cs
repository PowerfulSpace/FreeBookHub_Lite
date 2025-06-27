using FluentValidation;
using PS.FreeBookHub_Lite.AuthService.Application.Interfaces;

namespace PS.FreeBookHub_Lite.AuthService.Application.CQRS.Commands.Login
{
    public class LoginCommandValidator : AbstractValidator<LoginCommand>
    {
        public LoginCommandValidator(IUserRepository userRepository)
        {
            RuleFor(x => x.Email)
                .MustAsync(async (email, ct) =>
                {
                    var user = await userRepository.GetByEmailAsync(email, ct, asNoTracking: true);
                    return user != null;
                })
                .WithMessage("Invalid credentials.");

            RuleFor(x => x.Email)
                .MustAsync(async (email, ct) =>
                {
                    var user = await userRepository.GetByEmailAsync(email, ct, asNoTracking: true);
                    return user == null || user.IsActive;
                })
                .WithMessage("This user account has been deactivated.");
        }
    }
}

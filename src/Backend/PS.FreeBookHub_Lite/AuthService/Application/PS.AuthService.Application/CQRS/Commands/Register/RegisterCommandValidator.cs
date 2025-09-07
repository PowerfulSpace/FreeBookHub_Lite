using FluentValidation;
using PS.AuthService.Application.Interfaces;

namespace PS.AuthService.Application.CQRS.Commands.Register
{
    public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
    {
        public RegisterCommandValidator(IUserRepository userRepository)
        {
            RuleFor(x => x.Email)
                .MustAsync(async (email, ct) =>
                {
                    var existing = await userRepository.GetByEmailAsync(email, ct, asNoTracking: true);
                    return existing == null;
                })
                .WithMessage("A user with this email already exists.");
        }
    }
}

using FluentValidation;

namespace PS.FreeBookHub_Lite.AuthService.Application.CQRS.Commands.LogoutAll
{
    public class LogoutAllSessionsCommandValidator : AbstractValidator<LogoutAllSessionsCommand>
    {
        public LogoutAllSessionsCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("UserId is required.");
        }
    }
}

using FluentValidation;
using PS.FreeBookHub_Lite.AuthService.Application.Interfaces;

namespace PS.FreeBookHub_Lite.AuthService.Application.CQRS.Commands.Logout
{
    public class LogoutCommandValidator : AbstractValidator<LogoutCommand>
    {
        public LogoutCommandValidator(IRefreshTokenRepository refreshTokenRepository)
        {
            RuleFor(x => x.RefreshToken)
                .MustAsync(async (token, ct) =>
                {
                    var rt = await refreshTokenRepository.GetByTokenAsync(token, ct);
                    return rt != null;
                })
                .WithMessage("Refresh token not found.");

            RuleFor(x => x.RefreshToken)
                .MustAsync(async (token, ct) =>
                {
                    var rt = await refreshTokenRepository.GetByTokenAsync(token, ct);
                    return rt == null || rt.IsActive();
                })
                .WithMessage("Refresh token is already revoked.");
        }
    }
}

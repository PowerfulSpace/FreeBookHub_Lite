using FluentValidation;
using PS.FreeBookHub_Lite.AuthService.Application.Interfaces;

namespace PS.FreeBookHub_Lite.AuthService.Application.CQRS.Commands.Logout
{
    public class LogoutCommandValidator : AbstractValidator<LogoutCommand>
    {
        public LogoutCommandValidator(IRefreshTokenRepository refreshTokenRepository)
        {
            RuleFor(x => x.RefreshToken)
                .CustomAsync(async (token, context, ct) =>
                {
                    var rt = await refreshTokenRepository.GetByTokenAsync(token, ct);

                    if (rt == null)
                    {
                        context.AddFailure("RefreshToken", "Refresh token not found.");
                        return;
                    }

                    if (!rt.IsActive())
                    {
                        context.AddFailure("RefreshToken", "Refresh token is already revoked.");
                    }
                });
        }
    }
}
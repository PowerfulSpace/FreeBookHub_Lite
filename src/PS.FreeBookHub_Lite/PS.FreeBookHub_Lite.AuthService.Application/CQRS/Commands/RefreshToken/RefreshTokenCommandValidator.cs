using FluentValidation;
using PS.FreeBookHub_Lite.AuthService.Application.Interfaces;

namespace PS.FreeBookHub_Lite.AuthService.Application.CQRS.Commands.RefreshToken
{
    public class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
    {
        public RefreshTokenCommandValidator(
            IRefreshTokenRepository refreshTokenRepository,
            IUserRepository userRepository)
        {
            RuleFor(x => x.RefreshToken)
                .MustAsync(async (token, ct) =>
                {
                    var rt = await refreshTokenRepository.GetByTokenAsync(token, ct);
                    return rt != null && rt.IsActive();
                })
                .WithMessage("Invalid or expired refresh token.");

            RuleFor(x => x.RefreshToken)
                .MustAsync(async (token, ct) =>
                {
                    var rt = await refreshTokenRepository.GetByTokenAsync(token, ct);
                    if (rt == null) return true; // Already handled in the previous rule
                    var user = await userRepository.GetByIdAsync(rt.UserId, ct, asNoTracking: true);
                    return user != null && user.IsActive;
                })
                .WithMessage("Associated user account is not active.");
        }
    }
}

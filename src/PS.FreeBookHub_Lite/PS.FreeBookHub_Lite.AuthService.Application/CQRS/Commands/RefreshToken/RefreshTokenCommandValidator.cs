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
                    var rt = await refreshTokenRepository.GetByTokenAsync(token, ct, asNoTracking: true);
                    return rt != null && rt.IsActive();
                })
                .WithMessage("Invalid or expired refresh token.")

                .DependentRules(() =>
                {
                    RuleFor(x => x.RefreshToken)
                        .MustAsync(async (token, ct) =>
                        {
                            var rt = await refreshTokenRepository.GetByTokenAsync(token, ct, asNoTracking: true);
                            if (rt == null) return true; // теоретически не дойдём, но на всякий
                            var user = await userRepository.GetByIdAsync(rt.UserId, ct, asNoTracking: true);
                            return user != null && user.IsActive;
                        })
                        .WithMessage("Associated user account is not active.");
                });
        }
    }
}

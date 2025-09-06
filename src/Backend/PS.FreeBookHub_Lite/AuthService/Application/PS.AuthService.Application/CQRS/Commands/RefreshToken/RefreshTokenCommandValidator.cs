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
                .CustomAsync(async (token, context, ct) =>
                {
                    var rt = await refreshTokenRepository.GetByTokenAsync(token, ct, asNoTracking: true);

                    if (rt == null || !rt.IsActive())
                    {
                        context.AddFailure("RefreshToken", "Invalid or expired refresh token.");
                        return;
                    }

                    var user = await userRepository.GetByIdAsync(rt.UserId, ct, asNoTracking: true);
                    if (user == null || !user.IsActive)
                    {
                        context.AddFailure("RefreshToken", "Associated user account is not active.");
                    }
                });
        }
    }
}

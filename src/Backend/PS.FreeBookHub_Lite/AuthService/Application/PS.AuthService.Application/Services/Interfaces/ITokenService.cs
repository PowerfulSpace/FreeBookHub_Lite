using PS.AuthService.Domain.Entities;

namespace PS.AuthService.Application.Services.Interfaces
{
    public interface ITokenService
    {
        string GenerateAccessToken(User user);
        string GenerateRefreshToken();
    }
}

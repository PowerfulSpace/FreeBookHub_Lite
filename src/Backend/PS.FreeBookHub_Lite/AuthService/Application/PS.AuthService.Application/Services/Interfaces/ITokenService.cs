using PS.FreeBookHub_Lite.AuthService.Domain.Entities;

namespace PS.FreeBookHub_Lite.AuthService.Application.Services.Interfaces
{
    public interface ITokenService
    {
        string GenerateAccessToken(User user);
        string GenerateRefreshToken();
    }
}

using Microsoft.Extensions.Options;
using PS.FreeBookHub_Lite.AuthService.Domain.Entities;
using PS.FreeBookHub_Lite.AuthService.Infrastructure.Autentication;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace AuthService.UnitTests.Infrastructure.Autentication
{
    public class TokenServiceTests
    {
        private TokenService CreateService()
        {
            var jwtSettings = new JwtSettings
            {
                SecretKey = "super_secret_key_123456789012345", // минимум 256 бит для HMAC-SHA256
                Issuer = "TestIssuer",
                Audience = "TestAudience",
                AccessTokenExpiryMinutes = 30
            };

            var options = Options.Create(jwtSettings);
            return new TokenService(options);
        }

        [Fact]
        public void GenerateRefreshToken_ShouldReturn_NonEmpty_ValidToken()
        {
            var service = CreateService();

            var token = service.GenerateRefreshToken();

            Assert.False(string.IsNullOrWhiteSpace(token));
            Assert.True(token.Length >= 43); // Base64Url 32 bytes = ~43 chars
        }

        [Fact]
        public void GenerateAccessToken_ShouldContain_ExpectedClaims()
        {
            var service = CreateService();

            var user = new User("user@example.com", "hashedpassword");

            var token = service.GenerateAccessToken(user);

            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);

            Assert.Equal("TestIssuer", jwt.Issuer);
            Assert.Equal("TestAudience", jwt.Audiences.First());

            Assert.Contains(jwt.Claims, c => c.Type == ClaimTypes.Role && c.Value == "User");
            Assert.Contains(jwt.Claims, c => c.Type == JwtRegisteredClaimNames.Email && c.Value == user.Email);
            Assert.Contains(jwt.Claims, c => c.Type == "nameidentifier" && c.Value == user.Id.ToString());
            Assert.Contains(jwt.Claims, c => c.Type == JwtRegisteredClaimNames.Jti);
        }
    }
}

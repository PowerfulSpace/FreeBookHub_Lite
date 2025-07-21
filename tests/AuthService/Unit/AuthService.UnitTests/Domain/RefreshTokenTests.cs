using PS.FreeBookHub_Lite.AuthService.Domain.Entities;
using PS.FreeBookHub_Lite.AuthService.Domain.Exceptions.Token;

namespace AuthService.UnitTests.Domain
{
    public class RefreshTokenTests
    {
        [Fact]
        public void CreateToken_ShouldSetFieldsCorrectly()
        {
            var userId = Guid.NewGuid();
            var token = "some-token";
            var expiresAt = DateTime.UtcNow.AddMinutes(30);

            var refreshToken = new RefreshToken(userId, token, expiresAt);

            Assert.Equal(userId, refreshToken.UserId);
            Assert.Equal(token, refreshToken.Token);
            Assert.Equal(expiresAt, refreshToken.ExpiresAt);
            Assert.False(refreshToken.IsRevoked);
        }

        [Fact]
        public void Revoke_AlreadyRevoked_ShouldThrow()
        {
            var token = new RefreshToken(Guid.NewGuid(), "token", DateTime.UtcNow.AddMinutes(5));
            token.Revoke();

            var ex = Assert.Throws<RevokedTokenException>(() => token.Revoke());
            Assert.Equal("token", ex.Token);
        }

        [Fact]
        public void IsActive_WhenNotExpiredAndNotRevoked_ShouldBeTrue()
        {
            var token = new RefreshToken(Guid.NewGuid(), "token", DateTime.UtcNow.AddMinutes(5));

            Assert.True(token.IsActive());
        }

        [Fact]
        public void IsExpired_WhenPastExpiration_ShouldBeTrue()
        {
            var token = new RefreshToken(Guid.NewGuid(), "token", DateTime.UtcNow.AddMinutes(-1));

            Assert.True(token.IsExpired());
        }
    }
}

using PS.AuthService.Domain.Exceptions.Token;

namespace PS.AuthService.Domain.Entities
{
    public class RefreshToken
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public Guid UserId { get; private set; }
        public string Token { get; private set; } = null!;
        public DateTime ExpiresAt { get; private set; }
        public bool IsRevoked { get; private set; }
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

        protected RefreshToken() { }

        public RefreshToken(Guid userId, string token, DateTime expiresAt)
        {
            UserId = userId;
            Token = token;
            ExpiresAt = expiresAt;
            IsRevoked = false;
        }

        public void Revoke()
        {
            if (IsRevoked)
                throw new RevokedTokenException(Token);
            IsRevoked = true;
        }

        public bool IsExpired() => DateTime.UtcNow >= ExpiresAt;
        public bool IsActive() => !IsRevoked && !IsExpired();
    }
}

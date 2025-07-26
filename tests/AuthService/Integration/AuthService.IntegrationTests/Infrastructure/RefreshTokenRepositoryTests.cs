using AuthService.IntegrationTests.TestUtils.Factories;
using Microsoft.EntityFrameworkCore;
using PS.FreeBookHub_Lite.AuthService.Domain.Entities;
using PS.FreeBookHub_Lite.AuthService.Infrastructure.Persistence.Repositories;

namespace AuthService.IntegrationTests.Infrastructure
{
    public class RefreshTokenRepositoryTests
    {
        private readonly CancellationToken _ct = CancellationToken.None;

        [Fact]
        public async Task AddAsync_Should_Add_Token()
        {
            var context = SqliteTestDbFactory.Create();
            var repository = new RefreshTokenRepository(context);

            var token = new RefreshToken(
                userId: Guid.NewGuid(),
                token: "abc123",
                expiresAt: DateTime.UtcNow.AddDays(7)
                );

            await repository.AddAsync(token, _ct);

            var saved = await context.RefreshTokens.FindAsync(token.Id);
            Assert.NotNull(saved);
            Assert.Equal("abc123", saved!.Token);
        }

        [Fact]
        public async Task GetByTokenAsync_Should_Return_Correct_Token()
        {
            var context = SqliteTestDbFactory.Create();
            var repository = new RefreshTokenRepository(context);

            var token = new RefreshToken(
               userId: Guid.NewGuid(),
               token: "abc123",
               expiresAt: DateTime.UtcNow.AddDays(7)
               );

            context.RefreshTokens.Add(token);
            await context.SaveChangesAsync();

            var result = await repository.GetByTokenAsync("xyz456", _ct);

            Assert.NotNull(result);
            Assert.Equal(token.Id, result!.Id);
        }

        [Fact]
        public async Task UpdateAsync_Should_Modify_Token()
        {
            var context = SqliteTestDbFactory.Create();
            var repository = new RefreshTokenRepository(context);

            var originalToken = new RefreshToken(
                userId: Guid.NewGuid(),
                token: "original",
                expiresAt: DateTime.UtcNow.AddDays(7)
            );

            context.RefreshTokens.Add(originalToken);
            await context.SaveChangesAsync();
            context.Entry(originalToken).State = EntityState.Detached;

            // Новый экземпляр с тем же Id
            var updatedToken = new RefreshToken(
                userId: originalToken.UserId,
                token: "updated",
                expiresAt: originalToken.ExpiresAt
            );

            typeof(RefreshToken)
                .GetProperty("Id")!
                .SetValue(updatedToken, originalToken.Id);

            await repository.UpdateAsync(updatedToken, _ct);

            var result = await context.RefreshTokens.FindAsync(originalToken.Id);
            Assert.Equal("updated", result!.Token);
        }
        [Fact]
        public async Task GetActiveTokensByUserIdAsync_Should_Return_Only_Active()
        {
            var context = SqliteTestDbFactory.Create();
            var repository = new RefreshTokenRepository(context);

            var userId = Guid.NewGuid();

            var active = new RefreshToken(userId, "active", DateTime.UtcNow.AddDays(1));
            var expired = new RefreshToken(userId, "expired", DateTime.UtcNow.AddDays(-1));
            var revoked = new RefreshToken(userId, "revoked", DateTime.UtcNow.AddDays(1));
            revoked.Revoke();

            context.RefreshTokens.AddRange(active, expired, revoked);
            await context.SaveChangesAsync();

            var result = await repository.GetActiveTokensByUserIdAsync(userId, _ct);

            Assert.Single(result);
            Assert.Equal("active", result[0].Token);
        }

        [Fact]
        public async Task RevokeAllTokensForUserAsync_Should_Revoke_Active_Tokens()
        {
            var context = SqliteTestDbFactory.Create();
            var repository = new RefreshTokenRepository(context);

            var userId = Guid.NewGuid();

            var token1 = new RefreshToken(userId, "one", DateTime.UtcNow.AddDays(2));
            var token2 = new RefreshToken(userId, "two", DateTime.UtcNow.AddDays(3));
            var alreadyRevoked = new RefreshToken(userId, "revoked", DateTime.UtcNow.AddDays(3));
            alreadyRevoked.Revoke();

            context.RefreshTokens.AddRange(token1, token2, alreadyRevoked);
            await context.SaveChangesAsync();

            await repository.RevokeAllTokensForUserAsync(userId, _ct);

            var tokens = await context.RefreshTokens
                .Where(t => t.UserId == userId)
                .ToListAsync();

            Assert.All(tokens, t => Assert.True(t.IsRevoked));
        }
    }
}

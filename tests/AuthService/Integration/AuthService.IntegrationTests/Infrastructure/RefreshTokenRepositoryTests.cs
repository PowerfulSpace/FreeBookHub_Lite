using AuthService.IntegrationTests.TestUtils.Factories;
using Microsoft.EntityFrameworkCore;
using PS.FreeBookHub_Lite.AuthService.Domain.Entities;
using PS.FreeBookHub_Lite.AuthService.Domain.Enums;
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

            var user = CreateValidUser();
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var token = new RefreshToken(
                userId: user.Id,
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

            var user = CreateValidUser();
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var token = new RefreshToken(
               userId: user.Id,
               token: "abc123",
               expiresAt: DateTime.UtcNow.AddDays(7)
               );

            context.RefreshTokens.Add(token);
            await context.SaveChangesAsync();

            var result = await repository.GetByTokenAsync("abc123", _ct);

            Assert.NotNull(result);
            Assert.Equal(token.Id, result!.Id);
        }

        [Fact]
        public async Task UpdateAsync_Should_Modify_Token()
        {
            var context = SqliteTestDbFactory.Create();
            var repository = new RefreshTokenRepository(context);

            var user = CreateValidUser();
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var originalToken = new RefreshToken(
                userId: user.Id,
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

            var user = CreateValidUser();
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var active = new RefreshToken(user.Id, "active", DateTime.UtcNow.AddDays(1));
            var expired = new RefreshToken(user.Id, "expired", DateTime.UtcNow.AddDays(-1));
            var revoked = new RefreshToken(user.Id, "revoked", DateTime.UtcNow.AddDays(1));
            revoked.Revoke();

            context.RefreshTokens.AddRange(active, expired, revoked);
            await context.SaveChangesAsync();

            var result = await repository.GetActiveTokensByUserIdAsync(user.Id, _ct);

            Assert.Single(result);
            Assert.Equal("active", result[0].Token);
        }

        [Fact]
        public async Task RevokeAllTokensForUserAsync_Should_Revoke_Active_Tokens()
        {
            var context = SqliteTestDbFactory.Create();
            var repository = new RefreshTokenRepository(context);

            var user = CreateValidUser();
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var now = DateTime.UtcNow;

            var token1 = new RefreshToken(user.Id, "one", now.AddDays(2));
            var token2 = new RefreshToken(user.Id, "two", now.AddDays(3));
            var alreadyRevoked = new RefreshToken(user.Id, "revoked", now.AddDays(3));
            alreadyRevoked.Revoke();

            context.RefreshTokens.AddRange(token1, token2, alreadyRevoked);
            await context.SaveChangesAsync();

            await repository.RevokeAllTokensForUserAsync(user.Id, _ct);

            var tokens = await context.RefreshTokens
                .Where(t => t.UserId == user.Id)
                .ToListAsync();

            Assert.All(tokens, t => Assert.True(t.IsRevoked));
        }

        private static User CreateValidUser()
        {
            return new User(
                email: $"user_{Guid.NewGuid()}@test.com",
                passwordHash: "hashed_password",
                role: UserRole.User
            );
        }
    }
}

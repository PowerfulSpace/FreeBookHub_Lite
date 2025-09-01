using AuthService.IntegrationTests.TestUtils.Factories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PS.FreeBookHub_Lite.AuthService.Domain.Entities;
using PS.FreeBookHub_Lite.AuthService.Domain.Enums;
using PS.FreeBookHub_Lite.AuthService.Infrastructure.Persistence;
using PS.FreeBookHub_Lite.AuthService.Infrastructure.Persistence.Repositories;

namespace AuthService.IntegrationTests.Infrastructure
{
    public class RefreshTokenRepositoryTests : IClassFixture<AuthApiFactory>
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public RefreshTokenRepositoryTests(AuthApiFactory factory)
        {
            _scopeFactory = factory.Services.GetRequiredService<IServiceScopeFactory>();
        }

        private async Task ExecuteScopeAsync(Func<RefreshTokenRepository, AuthDbContext, Task> action)
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
            var repo = new RefreshTokenRepository(db);
            await action(repo, db);
        }

        [Fact]
        public async Task AddAsync_Should_Add_Token()
        {
            await ExecuteScopeAsync(async (repo, db) =>
            {
                var user = CreateValidUser();
                db.Users.Add(user);
                await db.SaveChangesAsync();

                var tokenValue = $"tok-{Guid.NewGuid()}";
                var token = new RefreshToken(user.Id, tokenValue, DateTime.UtcNow.AddDays(7));

                await repo.AddAsync(token, CancellationToken.None);

                var saved = await db.RefreshTokens.FindAsync(token.Id);
                Assert.NotNull(saved);
                Assert.Equal(tokenValue, saved!.Token);
            });
        }

        [Fact]
        public async Task GetByTokenAsync_Should_Return_Correct_Token()
        {
            await ExecuteScopeAsync(async (repo, db) =>
            {
                var user = CreateValidUser();
                db.Users.Add(user);
                await db.SaveChangesAsync();

                var tokenValue = $"tok-{Guid.NewGuid()}";
                var token = new RefreshToken(user.Id, tokenValue, DateTime.UtcNow.AddDays(7));
                db.RefreshTokens.Add(token);
                await db.SaveChangesAsync();

                var found = await repo.GetByTokenAsync(tokenValue, CancellationToken.None);

                Assert.NotNull(found);
                Assert.Equal(token.Id, found!.Id);
            });
        }

        [Fact]
        public async Task GetByTokenAsync_Should_Return_Null_If_Not_Exists()
        {
            await ExecuteScopeAsync(async (repo, db) =>
            {
                var notFound = await repo.GetByTokenAsync("no-such-token", CancellationToken.None);
                Assert.Null(notFound);
            });
        }

        [Fact]
        public async Task GetByTokenAsync_AsNoTracking_Should_Not_Track_Entity()
        {
            await ExecuteScopeAsync(async (repo, db) =>
            {
                var user = CreateValidUser();
                db.Users.Add(user);
                await db.SaveChangesAsync();

                var tokenValue = $"tok-{Guid.NewGuid()}";
                var token = new RefreshToken(user.Id, tokenValue, DateTime.UtcNow.AddDays(2));
                db.RefreshTokens.Add(token);
                await db.SaveChangesAsync();

                db.Entry(token).State = EntityState.Detached;

                var found = await repo.GetByTokenAsync(tokenValue, CancellationToken.None, asNoTracking: true);

                var isTracked = db.ChangeTracker.Entries<RefreshToken>().Any(e => e.Entity.Token == tokenValue);
                Assert.False(isTracked);
                Assert.NotNull(found);
            });
        }

        [Fact]
        public async Task UpdateAsync_Should_Modify_Token()
        {
            await ExecuteScopeAsync(async (repo, db) =>
            {
                var user = CreateValidUser();
                db.Users.Add(user);
                await db.SaveChangesAsync();

                var original = new RefreshToken(user.Id, $"tok-{Guid.NewGuid()}", DateTime.UtcNow.AddDays(7));
                db.RefreshTokens.Add(original);
                await db.SaveChangesAsync();

                // Отсоединяем и обновляем через новый инстанс с тем же Id
                db.Entry(original).State = EntityState.Detached;
                var newValue = $"tok-{Guid.NewGuid()}";
                var updated = new RefreshToken(user.Id, newValue, original.ExpiresAt);
                typeof(RefreshToken).GetProperty(nameof(RefreshToken.Id))!.SetValue(updated, original.Id);

                await repo.UpdateAsync(updated, CancellationToken.None);

                var refreshed = await repo.GetByTokenAsync(newValue, CancellationToken.None);
                Assert.NotNull(refreshed);
                Assert.Equal(updated.ExpiresAt, refreshed!.ExpiresAt);
            });
        }

        [Fact]
        public async Task GetActiveTokensByUserIdAsync_Should_Return_Only_Active()
        {
            await ExecuteScopeAsync(async (repo, db) =>
            {
                var user = CreateValidUser();
                db.Users.Add(user);
                await db.SaveChangesAsync();

                var now = DateTime.UtcNow;
                var active = new RefreshToken(user.Id, $"tok-active-{Guid.NewGuid()}", now.AddHours(12));
                var expired = new RefreshToken(user.Id, $"tok-expired-{Guid.NewGuid()}", now.AddHours(-1));
                var revoked = new RefreshToken(user.Id, $"tok-revoked-{Guid.NewGuid()}", now.AddHours(12));
                revoked.Revoke();

                db.RefreshTokens.AddRange(active, expired, revoked);
                await db.SaveChangesAsync();

                var result = await repo.GetActiveTokensByUserIdAsync(user.Id, CancellationToken.None);

                Assert.Single(result);
                Assert.Equal(active.Token, result[0].Token);
            });
        }

        [Fact]
        public async Task GetActiveTokensByUserIdAsync_Should_Return_Empty_If_All_RevokedOrExpired()
        {
            await ExecuteScopeAsync(async (repo, db) =>
            {
                var user = CreateValidUser();
                db.Users.Add(user);
                await db.SaveChangesAsync();

                var now = DateTime.UtcNow;
                var expired = new RefreshToken(user.Id, $"tok-expired-{Guid.NewGuid()}", now.AddHours(-2));
                var revoked = new RefreshToken(user.Id, $"tok-revoked-{Guid.NewGuid()}", now.AddHours(5));
                revoked.Revoke();

                db.RefreshTokens.AddRange(expired, revoked);
                await db.SaveChangesAsync();

                var result = await repo.GetActiveTokensByUserIdAsync(user.Id, CancellationToken.None);

                Assert.Empty(result);
            });
        }

        [Fact]
        public async Task RevokeAllTokensForUserAsync_Should_Revoke_Only_Active_Not_Expired()
        {
            await ExecuteScopeAsync(async (repo, db) =>
            {
                var user = CreateValidUser();
                db.Users.Add(user);
                await db.SaveChangesAsync();

                var now = DateTime.UtcNow;
                var active1 = new RefreshToken(user.Id, $"tok-a1-{Guid.NewGuid()}", now.AddHours(6));
                var active2 = new RefreshToken(user.Id, $"tok-a2-{Guid.NewGuid()}", now.AddHours(12));
                var expired = new RefreshToken(user.Id, $"tok-exp-{Guid.NewGuid()}", now.AddHours(-3));
                var alreadyRevoked = new RefreshToken(user.Id, $"tok-rev-{Guid.NewGuid()}", now.AddHours(8));
                alreadyRevoked.Revoke();

                db.RefreshTokens.AddRange(active1, active2, expired, alreadyRevoked);
                await db.SaveChangesAsync();

                await repo.RevokeAllTokensForUserAsync(user.Id, CancellationToken.None);

                var tokens = await db.RefreshTokens.Where(t => t.UserId == user.Id).ToListAsync();

                // активные → revoked
                Assert.True(tokens.First(t => t.Id == active1.Id).IsRevoked);
                Assert.True(tokens.First(t => t.Id == active2.Id).IsRevoked);
                // уже отозванный остался отозванным
                Assert.True(tokens.First(t => t.Id == alreadyRevoked.Id).IsRevoked);
                // просроченный не трогаем
                Assert.False(tokens.First(t => t.Id == expired.Id).IsRevoked);
            });
        }

        private static User CreateValidUser() =>
            new User(email: $"user_{Guid.NewGuid()}@test.com", passwordHash: "hashed_password", role: UserRole.User);
    }
}

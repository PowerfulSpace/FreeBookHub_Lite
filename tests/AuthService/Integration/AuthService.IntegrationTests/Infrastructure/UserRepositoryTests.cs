using AuthService.IntegrationTests.TestUtils.Factories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PS.FreeBookHub_Lite.AuthService.Domain.Entities;
using PS.FreeBookHub_Lite.AuthService.Domain.Enums;
using PS.FreeBookHub_Lite.AuthService.Infrastructure.Persistence;
using PS.FreeBookHub_Lite.AuthService.Infrastructure.Persistence.Repositories;

namespace AuthService.IntegrationTests.Infrastructure
{
    public class UserRepositoryTests : IClassFixture<AuthApiFactory>
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public UserRepositoryTests(AuthApiFactory factory)
        {
            _scopeFactory = factory.Services.GetRequiredService<IServiceScopeFactory>();
        }

        private async Task ExecuteScopeAsync(Func<UserRepository, AuthDbContext, Task> action)
        {
            using var scope = _scopeFactory.CreateScope();
            var repo = new UserRepository(scope.ServiceProvider.GetRequiredService<AuthDbContext>());
            var db = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
            await action(repo, db);
        }

        [Fact]
        public async Task AddAsync_Should_Add_User_And_GetByIdAsync_Should_Return_It()
        {
            await ExecuteScopeAsync(async (repo, db) =>
            {
                var user = new User("userrepo@example.com", "hashed-password");

                await repo.AddAsync(user, CancellationToken.None);

                var found = await repo.GetByIdAsync(user.Id, CancellationToken.None);

                Assert.NotNull(found);
                Assert.Equal("userrepo@example.com", found!.Email);
                Assert.Equal("hashed-password", found.PasswordHash);
                Assert.Equal(UserRole.User, found.Role);
                Assert.True(found.IsActive);
            });
        }

        [Fact]
        public async Task GetByEmailAsync_Should_Return_User_When_Exists()
        {
            await ExecuteScopeAsync(async (repo, db) =>
            {
                var user = new User("byemail@example.com", "hash");
                db.Users.Add(user);
                await db.SaveChangesAsync();

                var found = await repo.GetByEmailAsync("byemail@example.com", CancellationToken.None);

                Assert.NotNull(found);
                Assert.Equal(user.Id, found!.Id);
            });
        }

        [Fact]
        public async Task UpdateAsync_Should_Update_User_Data()
        {
            await ExecuteScopeAsync(async (repo, db) =>
            {
                var user = new User("update@example.com", "old-hash");
                db.Users.Add(user);
                await db.SaveChangesAsync();

                // создаём новый объект с тем же Id (Attach + Modified в репо)
                var updatedUser = new User("update@example.com", "new-hash");
                typeof(User).GetProperty(nameof(User.Id))!
                    .SetValue(updatedUser, user.Id); // форсим Id через reflection

                await repo.UpdateAsync(updatedUser, CancellationToken.None);

                var refreshed = await repo.GetByIdAsync(user.Id, CancellationToken.None);

                Assert.Equal("new-hash", refreshed!.PasswordHash);
            });
        }

        [Fact]
        public async Task DeleteAsync_Should_Remove_User()
        {
            await ExecuteScopeAsync(async (repo, db) =>
            {
                var user = new User("delete@example.com", "hash");
                db.Users.Add(user);
                await db.SaveChangesAsync();

                await repo.DeleteAsync(user.Id, CancellationToken.None);

                var found = await repo.GetByIdAsync(user.Id, CancellationToken.None);

                Assert.Null(found);
            });
        }

        [Fact]
        public async Task GetByIdAsync_Should_Return_Null_When_Not_Found()
        {
            await ExecuteScopeAsync(async (repo, db) =>
            {
                var notFound = await repo.GetByIdAsync(Guid.NewGuid(), CancellationToken.None);
                Assert.Null(notFound);
            });
        }

        [Fact]
        public async Task GetByIdAsync_AsNoTracking_Should_Not_Track_Entity()
        {
            await ExecuteScopeAsync(async (repo, db) =>
            {
                var user = new User("ntracking@example.com", "hash");
                db.Users.Add(user);
                await db.SaveChangesAsync();

                var found = await repo.GetByIdAsync(user.Id, CancellationToken.None, asNoTracking: true);

                var isTracked = db.ChangeTracker.Entries<User>().Any(e => e.Entity.Id == user.Id);
                Assert.False(isTracked);
                Assert.NotNull(found);
            });
        }

        [Fact]
        public async Task AddAsync_Should_Throw_When_Duplicate_Id()
        {
            await ExecuteScopeAsync(async (repo, db) =>
            {
                var user1 = new User("dup@example.com", "hash");
                await repo.AddAsync(user1, CancellationToken.None);

                var user2 = new User("dup2@example.com", "hash");
                typeof(User).GetProperty(nameof(User.Id))!
                    .SetValue(user2, user1.Id);

                await Assert.ThrowsAsync<DbUpdateException>(() => repo.AddAsync(user2, CancellationToken.None));
            });
        }

        [Fact]
        public async Task UpdateAsync_Should_Change_Email_And_Role()
        {
            await ExecuteScopeAsync(async (repo, db) =>
            {
                var user = new User("rolechange@example.com", "hash");
                db.Users.Add(user);
                await db.SaveChangesAsync();

                // создаём новый объект с тем же Id и другими данными
                var updatedUser = new User("rolechanged@example.com", "hash", UserRole.Admin);
                typeof(User).GetProperty(nameof(User.Id))!
                    .SetValue(updatedUser, user.Id);

                await repo.UpdateAsync(updatedUser, CancellationToken.None);

                var refreshed = await repo.GetByIdAsync(user.Id, CancellationToken.None);

                Assert.Equal("rolechanged@example.com", refreshed!.Email);
                Assert.Equal(UserRole.Admin, refreshed.Role);
            });
        }

        [Fact]
        public async Task DeleteAsync_Should_Not_Throw_When_Id_Not_Exists()
        {
            await ExecuteScopeAsync(async (repo, db) =>
            {
                var randomId = Guid.NewGuid();

                // Должно пройти тихо, без исключения
                await repo.DeleteAsync(randomId, CancellationToken.None);

                var stillNotFound = await repo.GetByIdAsync(randomId, CancellationToken.None);
                Assert.Null(stillNotFound);
            });
        }
    }
}
using AuthService.IntegrationTests.TestUtils.Factories;
using Microsoft.EntityFrameworkCore;
using PS.FreeBookHub_Lite.AuthService.Domain.Entities;
using PS.FreeBookHub_Lite.AuthService.Domain.Enums;
using PS.FreeBookHub_Lite.AuthService.Infrastructure.Persistence.Repositories;

namespace AuthService.IntegrationTests.Infrastructure
{
    public class UserRepositoryTests
    {
        private readonly CancellationToken _ct = CancellationToken.None;

        [Fact]
        public async Task AddAsync_Should_Add_User_To_Database()
        {
            // Arrange
            var context = InMemoryTestDbFactory.Create();
            var repository = new UserRepository(context);

            var user = new User("test@example.com", "hashed123");

            // Act
            await repository.AddAsync(user, _ct);

            // Assert
            var saved = await context.Users.FindAsync(user.Id);
            Assert.NotNull(saved);
            Assert.Equal("test@example.com", saved.Email);
        }

        [Fact]
        public async Task GetByIdAsync_Should_Return_Correct_User()
        {
            // Arrange
            var context = InMemoryTestDbFactory.Create();
            var repository = new UserRepository(context);

            // Act
            var user = new User("byid@example.com", "pass");
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var found = await repository.GetByIdAsync(user.Id, _ct);

            // Assert
            Assert.NotNull(found);
            Assert.Equal(user.Id, found!.Id);
        }

        [Fact]
        public async Task GetByEmailAsync_Should_Return_Correct_User()
        {
            // Arrange
            var context = InMemoryTestDbFactory.Create();
            var repository = new UserRepository(context);

            var user = new User("byemail@example.com", "pass");
            context.Users.Add(user);
            await context.SaveChangesAsync();

            // Act
            var found = await repository.GetByEmailAsync("byemail@example.com", _ct);

            // Assert
            Assert.NotNull(found);
            Assert.Equal(user.Email, found!.Email);
        }

        [Fact]
        public async Task UpdateAsync_Should_Modify_User()
        {
            // Arrange
            var context = InMemoryTestDbFactory.Create();
            var repository = new UserRepository(context);

            var user = new User("update@example.com", "oldHash");
            context.Users.Add(user);
            await context.SaveChangesAsync();
            context.Entry(user).State = EntityState.Detached;

            // Act
            var newUser = new User("new@example.com", "newHash", UserRole.Admin);
            typeof(User).GetProperty("Id")!.SetValue(newUser, user.Id); // сохраним тот же ID

            await repository.UpdateAsync(newUser, _ct);

            // Assert
            var updated = await context.Users.FindAsync(user.Id);
            Assert.NotNull(updated);
            Assert.Equal("new@example.com", updated!.Email);
        }

        [Fact]
        public async Task DeleteAsync_Should_Remove_User()
        {
            // Arrange
            var context = SqliteTestDbFactory.Create();
            var repository = new UserRepository(context);

            var user = new User("delete@example.com", "pass");
            context.Users.Add(user);
            await context.SaveChangesAsync();

            // Act
            await repository.DeleteAsync(user.Id, _ct);

            // Assert
            var deleted = await context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == user.Id, _ct);

            Assert.Null(deleted);
        }


        [Fact]
        public async Task GetByIdAsync_AsNoTracking_Should_Not_Track_Entity()
        {
            // Arrange
            var context = InMemoryTestDbFactory.Create();
            var repository = new UserRepository(context);

            var user = new User("ntracking@example.com", "pass");
            context.Users.Add(user);
            await context.SaveChangesAsync();

            // Act
            var found = await repository.GetByIdAsync(user.Id, _ct, asNoTracking: true);

            // Assert
            var isTracked = context.ChangeTracker.Entries<User>().Any(e => e.Entity.Id == user.Id);
            Assert.False(isTracked);
        }

        [Fact]
        public async Task GetByIdAsync_Should_Return_Null_If_Not_Found()
        {
            var context = InMemoryTestDbFactory.Create();
            var repository = new UserRepository(context);

            var result = await repository.GetByIdAsync(Guid.NewGuid(), _ct);

            Assert.Null(result);
        }

        [Fact]
        public async Task AddAsync_Should_Throw_On_Duplicate_Id()
        {
            var context = InMemoryTestDbFactory.Create();
            var repository = new UserRepository(context);

            var user = new User("duplicate@example.com", "pass");
            await repository.AddAsync(user, _ct);

            var duplicate = new User("another@example.com", "pass");
            typeof(User).GetProperty("Id")!.SetValue(duplicate, user.Id);

            await Assert.ThrowsAsync<DbUpdateException>(() => repository.AddAsync(duplicate, _ct));
        }
    }
}
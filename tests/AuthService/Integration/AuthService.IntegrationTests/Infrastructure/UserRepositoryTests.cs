using AuthService.IntegrationTests.TestUtils;
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
            var context = TestDbContextFactory.Create();
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
            var context = TestDbContextFactory.Create();
            var repository = new UserRepository(context);

            var user = new User("byid@example.com", "pass");
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var found = await repository.GetByIdAsync(user.Id, _ct);

            Assert.NotNull(found);
            Assert.Equal(user.Id, found!.Id);
        }

        [Fact]
        public async Task GetByEmailAsync_Should_Return_Correct_User()
        {
            var context = TestDbContextFactory.Create();
            var repository = new UserRepository(context);

            var user = new User("byemail@example.com", "pass");
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var found = await repository.GetByEmailAsync("byemail@example.com", _ct);

            Assert.NotNull(found);
            Assert.Equal(user.Email, found!.Email);
        }

        [Fact]
        public async Task UpdateAsync_Should_Modify_User()
        {
            var context = TestDbContextFactory.Create();
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
            var context = TestDbContextFactory.Create();
            var repository = new UserRepository(context);

            var user = new User("delete@example.com", "pass");
            context.Users.Add(user);
            await context.SaveChangesAsync();

            // Act
            await repository.DeleteAsync(user.Id, _ct);

            // Assert
            var deleted = await context.Users.FindAsync(user.Id);
            Assert.Null(deleted);
        }
    }
}
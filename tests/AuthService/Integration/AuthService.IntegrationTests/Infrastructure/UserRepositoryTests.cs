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
        private readonly CancellationToken _ct = CancellationToken.None;
        private readonly AuthDbContext _context;
        private readonly UserRepository _repository;

        public UserRepositoryTests(AuthApiFactory factory)
        {
            // Берём DbContext из фабрики
            _context = factory.Services.CreateScope().ServiceProvider.GetRequiredService<AuthDbContext>();
            _repository = new UserRepository(_context);
        }

        [Fact]
        public async Task AddAsync_Should_Add_User_To_Database()
        {
            var user = new User("test@example.com", "hashed123");

            await _repository.AddAsync(user, _ct);

            var saved = await _context.Users.FindAsync(user.Id);
            Assert.NotNull(saved);
            Assert.Equal("test@example.com", saved.Email);
        }

        [Fact]
        public async Task GetByIdAsync_Should_Return_Correct_User()
        {
            var user = new User("byid@example.com", "pass");
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var found = await _repository.GetByIdAsync(user.Id, _ct);

            Assert.NotNull(found);
            Assert.Equal(user.Id, found!.Id);
        }

        [Fact]
        public async Task GetByEmailAsync_Should_Return_Correct_User()
        {
            var user = new User("byemail@example.com", "pass");
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var found = await _repository.GetByEmailAsync("byemail@example.com", _ct);

            Assert.NotNull(found);
            Assert.Equal(user.Email, found!.Email);
        }

        [Fact]
        public async Task UpdateAsync_Should_Modify_User()
        {
            var user = new User("update@example.com", "oldHash");
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            _context.Entry(user).State = EntityState.Detached;

            var newUser = new User("new@example.com", "newHash", UserRole.Admin);
            typeof(User).GetProperty("Id")!.SetValue(newUser, user.Id);

            await _repository.UpdateAsync(newUser, _ct);

            var updated = await _context.Users.FindAsync(user.Id);
            Assert.NotNull(updated);
            Assert.Equal("new@example.com", updated!.Email);
        }

        [Fact]
        public async Task DeleteAsync_Should_Remove_User()
        {
            var user = new User("delete@example.com", "pass");
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            await _repository.DeleteAsync(user.Id, _ct);

            var deleted = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == user.Id, _ct);

            Assert.Null(deleted);
        }

        [Fact]
        public async Task GetByIdAsync_AsNoTracking_Should_Not_Track_Entity()
        {
            var user = new User("ntracking@example.com", "pass");
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var found = await _repository.GetByIdAsync(user.Id, _ct, asNoTracking: true);

            var isTracked = _context.ChangeTracker.Entries<User>().Any(e => e.Entity.Id == user.Id);
            Assert.False(isTracked);
            Assert.NotNull(found);
        }

        [Fact]
        public async Task GetByIdAsync_Should_Return_Null_If_Not_Found()
        {
            var result = await _repository.GetByIdAsync(Guid.NewGuid(), _ct);
            Assert.Null(result);
        }

        [Fact]
        public async Task AddAsync_Should_Throw_On_Duplicate_Id()
        {
            var user = new User("duplicate@example.com", "pass");
            await _repository.AddAsync(user, _ct);

            var duplicate = new User("another@example.com", "pass");
            typeof(User).GetProperty("Id")!.SetValue(duplicate, user.Id);

            await Assert.ThrowsAsync<DbUpdateException>(() => _repository.AddAsync(duplicate, _ct));
        }
    }
}
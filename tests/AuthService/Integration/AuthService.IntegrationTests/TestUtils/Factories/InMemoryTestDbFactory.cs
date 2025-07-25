using Microsoft.EntityFrameworkCore;
using PS.FreeBookHub_Lite.AuthService.Infrastructure.Persistence;

namespace AuthService.IntegrationTests.TestUtils.Factories
{
    public static class InMemoryTestDbFactory
    {
        public static AuthDbContext Create()
        {
            var options = new DbContextOptionsBuilder<AuthDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // уникальная база на каждый тест
                .Options;

            return new AuthDbContext(options);
        }
    }
}

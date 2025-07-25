using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using PS.FreeBookHub_Lite.AuthService.Infrastructure.Persistence;

namespace AuthService.IntegrationTests.TestUtils.Factories
{
    public static class SqliteTestDbFactory
    {
        public static AuthDbContext Create()
        {
            // Создаём открытое подключение SQLite In-Memory
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            var options = new DbContextOptionsBuilder<AuthDbContext>()
                .UseSqlite(connection)
                .EnableSensitiveDataLogging() // по желанию: помогает в отладке
                .Options;

            var context = new AuthDbContext(options);

            // Применяем миграции или EnsureCreated
            context.Database.EnsureCreated();

            return context;
        }
    }
}


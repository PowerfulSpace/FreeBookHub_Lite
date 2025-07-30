using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PS.FreeBookHub_Lite.AuthService.Infrastructure.Persistence;

namespace AuthService.IntegrationTests.TestUtils.Factories
{
    public class AuthApiFactory : WebApplicationFactory<Program>
    {
        private readonly SqliteConnection _connection = new("DataSource=:memory:");

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Удалим оригинальный DbContext
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<AuthDbContext>));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Открываем соединение заранее и регистрируем его
                _connection.Open();

                services.AddDbContext<AuthDbContext>(options =>
                {
                    options.UseSqlite(_connection); // <-- ВАЖНО!
                });

                // Собираем провайдер и вызываем EnsureCreated
                var sp = services.BuildServiceProvider();

                using var scope = sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
                db.Database.EnsureCreated();
            });
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            _connection.Close(); // Закрываем соединение при завершении тестов
        }
    }
}

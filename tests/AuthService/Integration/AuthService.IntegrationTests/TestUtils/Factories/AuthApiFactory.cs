using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using PS.FreeBookHub_Lite.AuthService.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace AuthService.IntegrationTests.TestUtils.Factories
{
    public class AuthApiFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Удалим оригинальный контекст
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<AuthDbContext>));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Добавим InMemory или SQLite (в зависимости от твоего выбора)
                services.AddDbContext<AuthDbContext>(options =>
                {
                    options.UseSqlite("DataSource=:memory:");
                });

                // Создадим базу и применим миграции
                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
                db.Database.OpenConnection();
                db.Database.EnsureCreated();
            });
        }
    }
}

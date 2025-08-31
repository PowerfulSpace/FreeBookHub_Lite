using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using PS.FreeBookHub_Lite.AuthService.Common.Interfaces.StartupTasks;
using PS.FreeBookHub_Lite.AuthService.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AuthService.IntegrationTests.TestUtils.Factories
{
    public class AuthApiFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // УДАЛЯЕМ StartupTask чтобы избежать миграций продакшен-базы
                services.RemoveAll<IStartupTask>();

                // Удаляем оригинальный DbContext
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<AuthDbContext>));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Подключаемся к тестовой MSSQL
                services.AddDbContext<AuthDbContext>(options =>
                {
                    options.UseSqlServer("Server=localhost,1434;Database=AuthTest;User=sa;Password=TestPassword123!;TrustServerCertificate=true;");
                });

                // Применяем миграции как в продакшене
                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AuthDbContext>();

                var retryPolicy = Policy
                    .Handle<SqlException>()
                    .WaitAndRetry(5, attempt => TimeSpan.FromSeconds(1));

                retryPolicy.Execute(() =>
                {
                    db.Database.EnsureDeleted();
                    db.Database.Migrate();
                });
            });
        }
    }
}

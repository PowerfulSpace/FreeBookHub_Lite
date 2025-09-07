using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using PS.AuthService.Infrastructure.Persistence;
using System.Collections.Concurrent;

namespace PS.AuthService.IntegrationTests.TestUtils.Factories
{
    public class AuthApiFactory : WebApplicationFactory<Program>
    {
        private static readonly ConcurrentDictionary<string, bool> _usedDatabases = new();
        private readonly string _databaseName;

        public AuthApiFactory()
        {
            _databaseName = $"AuthTest_{Guid.NewGuid():N}";
            while (_usedDatabases.ContainsKey(_databaseName))
            {
                _databaseName = $"AuthTest_{Guid.NewGuid():N}";
            }
            _usedDatabases[_databaseName] = true;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            // Добавляем конфигурацию для тестов

            builder.ConfigureAppConfiguration(config =>
            {
                config.AddInMemoryCollection(new Dictionary<string, string>
                {
                    ["ConnectionStrings:AuthDb"] =
                        $"Server=localhost,1434;Database={_databaseName};User=sa;Password=TestPassword123!;TrustServerCertificate=true",
                    ["Auth:JwtSettings:SecretKey"] = "my-256-bit-secret-key-is-secure!"
                }.Select(kvp => new KeyValuePair<string, string?>(kvp.Key, kvp.Value)));
            });

            builder.ConfigureServices(services =>
            {
                // Удаляем оригинальный DbContext
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<AuthDbContext>));
                if (descriptor != null)
                    services.Remove(descriptor);

                // Подключаем тестовую базу
                services.AddDbContext<AuthDbContext>(options =>
                {
                    options.UseSqlServer(
                        $"Server=localhost,1434;Database={_databaseName};User=sa;Password=TestPassword123!;TrustServerCertificate=true");
                });

                // Применяем миграции с retry
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

        protected override void Dispose(bool disposing)
        {
            _usedDatabases.TryRemove(_databaseName, out _);
            base.Dispose(disposing);
        }
    }
}

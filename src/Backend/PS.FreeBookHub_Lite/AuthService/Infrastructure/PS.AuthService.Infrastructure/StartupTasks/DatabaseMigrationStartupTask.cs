using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using PS.AuthService.Common.Interfaces.StartupTasks;
using PS.AuthService.Infrastructure.Persistence;

namespace PS.AuthService.Infrastructure.StartupTasks
{
    public class DatabaseMigrationStartupTask : StartupTask
    {
        private readonly IServiceProvider _serviceProvider;

        public DatabaseMigrationStartupTask(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AuthDbContext>();

            //await db.Database.MigrateAsync();

            var retryPolicy = Policy
                 .Handle<SqlException>()
                 .Or<InvalidOperationException>() // если соединение ещё не готово
                 .WaitAndRetryAsync(
                     retryCount: 5,
                     sleepDurationProvider: attempt => TimeSpan.FromSeconds(5));

            await retryPolicy.ExecuteAsync(() => db.Database.MigrateAsync());
        }
    }
}

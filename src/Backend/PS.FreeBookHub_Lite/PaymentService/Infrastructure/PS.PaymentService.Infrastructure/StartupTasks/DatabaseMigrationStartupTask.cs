using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using PS.PaymentService.Common.Interfaces.StartupTasks;
using PS.PaymentService.Infrastructure.Persistence;

namespace PS.PaymentService.Infrastructure.StartupTasks
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
            var db = scope.ServiceProvider.GetRequiredService<PaymentDbContext>();

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

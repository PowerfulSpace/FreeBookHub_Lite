using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PS.FreeBookHub_Lite.OrderService.Common.Interfaces.StartupTasks;
using PS.FreeBookHub_Lite.OrderService.Infrastructure.Persistence;

namespace PS.FreeBookHub_Lite.OrderService.Infrastructure.StartupTasks
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
            var db = scope.ServiceProvider.GetRequiredService<OrderDbContext>();

            await db.Database.MigrateAsync();
        }
    }
}

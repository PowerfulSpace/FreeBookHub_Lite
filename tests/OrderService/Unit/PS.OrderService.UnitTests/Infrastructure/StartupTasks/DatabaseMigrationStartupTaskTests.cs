using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PS.OrderService.Infrastructure.Persistence;
using PS.OrderService.Infrastructure.StartupTasks;

namespace PS.OrderService.UnitTests.Infrastructure.StartupTasks
{
    public class DatabaseMigrationStartupTaskTests
    {
        
    }

    public class TestableDatabaseMigrationStartupTask : DatabaseMigrationStartupTask
    {
        public TestableDatabaseMigrationStartupTask(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
        }

        public Task ExecutePublicAsync() => ExecuteAsync();
    }
}

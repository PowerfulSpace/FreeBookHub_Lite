using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PS.OrderService.Infrastructure.Persistence;
using PS.OrderService.Infrastructure.StartupTasks;

namespace PS.OrderService.UnitTests.Infrastructure.StartupTasks
{
    public class DatabaseMigrationStartupTaskTests
    {
        [Fact]
        public async Task ExecuteAsync_ShouldCompleteSuccessfully()
        {
            var services = new ServiceCollection();

            services.AddDbContext<OrderDbContext>(options =>
                options.UseInMemoryDatabase("TestDb1"));

            var provider = services.BuildServiceProvider();

            var task = new TestableDatabaseMigrationStartupTask(provider);

            var exception = await Record.ExceptionAsync(() => task.ExecutePublicAsync());

            Assert.Null(exception);
        }
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
using Microsoft.Extensions.DependencyInjection;
using PS.OrderService.Common.Interfaces.StartupTasks;

namespace PS.OrderService.Common.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddStartupTask<T>(this IServiceCollection services)
            where T : class, IStartupTask
        {
            return services.AddTransient<IStartupTask, T>();
        }
    }
}

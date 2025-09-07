using Microsoft.Extensions.DependencyInjection;
using PS.CatalogService.Common.Interfaces.StartupTasks;

namespace PS.CatalogService.Common.Extensions.DependencyInjection
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

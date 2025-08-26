using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PS.FreeBookHub_Lite.CatalogService.Common.Interfaces.StartupTasks;

namespace PS.FreeBookHub_Lite.CatalogService.Common.Extensions.Hosting
{
    public static class HostExtensions
    {
        public static IHost RunStartupTasks(this IHost host)
        {
            var startupTasks = host.Services.GetServices<IStartupTask>();

            foreach (var startupTask in startupTasks)
            {
                startupTask.Execute();
            }

            return host;
        }
    }
}

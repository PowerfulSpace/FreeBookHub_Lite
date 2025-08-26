using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PS.FreeBookHub_Lite.OrderService.Common.Interfaces.StartupTasks;

namespace PS.FreeBookHub_Lite.OrderService.Common.Extensions.Hosting
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

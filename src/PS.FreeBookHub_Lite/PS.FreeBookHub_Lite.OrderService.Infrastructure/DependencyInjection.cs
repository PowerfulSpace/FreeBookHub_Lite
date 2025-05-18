using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PS.FreeBookHub_Lite.OrderService.Application.Interfaces;
using PS.FreeBookHub_Lite.OrderService.Infrastructure.Persistence;
using PS.FreeBookHub_Lite.OrderService.Infrastructure.Persistence.Repositories;

namespace PS.FreeBookHub_Lite.OrderService.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, ConfigurationManager configuration)
        {
            services
                .AddPersistance(configuration);

            return services;
        }

        private static IServiceCollection AddPersistance(this IServiceCollection services, ConfigurationManager configuration)
        {
            services.AddDbContext<OrderDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("OrderDb")));

            services.AddScoped<IOrderRepository, OrderRepository>();

            return services;
        }
    }
}

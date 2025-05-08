using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PS.FreeBookHub_Lite.CartService.Application.Interfaces;
using PS.FreeBookHub_Lite.CartService.Infrastructure.Persistence;
using PS.FreeBookHub_Lite.CartService.Infrastructure.Persistence.Repositories;

namespace PS.FreeBookHub_Lite.CartService.Infrastructure
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
            services.AddDbContext<CartDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("CartDb")));

            services.AddScoped<ICartRepository, CartRepository>();

            return services;
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PS.FreeBookHub_Lite.CartService.Application.Clients;
using PS.FreeBookHub_Lite.CartService.Application.Interfaces;
using PS.FreeBookHub_Lite.CartService.Application.Services;
using PS.FreeBookHub_Lite.CartService.Infrastructure.Persistence;
using PS.FreeBookHub_Lite.CartService.Infrastructure.Persistence.Clients;
using PS.FreeBookHub_Lite.CartService.Infrastructure.Persistence.Repositories;

namespace PS.FreeBookHub_Lite.CartService.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, ConfigurationManager configuration)
        {
            services
                .AddPersistance(configuration)
                .AddHttpClients(configuration);

            services.AddHttpContextAccessor();

            return services;
        }

        private static IServiceCollection AddPersistance(this IServiceCollection services, ConfigurationManager configuration)
        {
            services.AddDbContext<CartDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("CartDb")));

            services.AddScoped<ICartRepository, CartRepository>();

            return services;
        }

        private static IServiceCollection AddHttpClients(this IServiceCollection services, ConfigurationManager configuration)
        {
            services.AddHttpClient<IOrderServiceClient, OrderServiceClient>(client =>
            {
                client.BaseAddress = new Uri(configuration["OrderService:BaseUrl"] ?? "https://localhost:7176");
            });

            services.AddHttpClient<IBookCatalogClient, BookCatalogClient>(client =>
            {
                client.BaseAddress = new Uri(configuration["CatalogService:BaseUrl"] ?? "https://localhost:7159");
            });

            return services;
        }
    }
}

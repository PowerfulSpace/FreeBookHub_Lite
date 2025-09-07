using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PS.CartService.Application.Clients;
using PS.CartService.Application.Interfaces;
using PS.CartService.Application.Security;
using PS.CartService.Infrastructure.Clients;
using PS.CartService.Infrastructure.Http.Handlers;
using PS.CartService.Infrastructure.Persistence;
using PS.CartService.Infrastructure.Persistence.Repositories;
using PS.CartService.Infrastructure.Security;

namespace PS.CartService.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, ConfigurationManager configuration)
        {
            services
                .AddPersistance(configuration)
                .AddHttpClients(configuration);

            services.AddHttpContextAccessor();

            services.AddTransient<AccessTokenHandler>();
            services.AddTransient<InternalAuthHandler>();

            services.AddScoped<IAccessTokenProvider, HttpContextAccessTokenProvider>();

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
            })
            .AddHttpMessageHandler<AccessTokenHandler>()
            .AddHttpMessageHandler<InternalAuthHandler>();

            services.AddHttpClient<IBookCatalogClient, BookCatalogClient>(client =>
            {
                client.BaseAddress = new Uri(configuration["CatalogService:BaseUrl"] ?? "https://localhost:7159");
            })
            .AddHttpMessageHandler<AccessTokenHandler>();

            return services;
        }
    }
}

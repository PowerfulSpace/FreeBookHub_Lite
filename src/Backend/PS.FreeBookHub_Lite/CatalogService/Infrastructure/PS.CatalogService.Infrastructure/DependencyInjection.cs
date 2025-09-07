using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PS.CatalogService.Application.Interfaces;
using PS.CatalogService.Infrastructure.Persistence;
using PS.CatalogService.Infrastructure.Persistence.Repositories;

namespace PS.CatalogService.Infrastructure
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
            services.AddDbContext<CatalogDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("CatalogDb")));

            services.AddScoped<IBookRepository, BookRepository>();

            return services;
        }
    }
}

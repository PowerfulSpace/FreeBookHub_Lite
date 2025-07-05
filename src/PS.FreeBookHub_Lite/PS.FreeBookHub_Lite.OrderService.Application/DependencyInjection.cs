using FluentValidation;
using FluentValidation.AspNetCore;
using Mapster;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PS.FreeBookHub_Lite.OrderService.Application.Mappings;
using PS.FreeBookHub_Lite.OrderService.Common.Configuration;

namespace PS.FreeBookHub_Lite.OrderService.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, ConfigurationManager configuration)
        {
            services
               .AddApplicationConfiguration(configuration)
               .AddApplicationMediatR()
               .AddApplicationValidation()
               .AddApplicationMapping();

            return services;
        }

        private static IServiceCollection AddApplicationConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<RabbitMqConfig>(configuration.GetSection("RabbitMQ"));
            return services;
        }


        private static IServiceCollection AddApplicationMediatR(this IServiceCollection services)
        {
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
            });

            return services;
        }

        private static IServiceCollection AddApplicationValidation(this IServiceCollection services)
        {
            services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

            services.AddFluentValidationAutoValidation(options =>
            {
                options.DisableDataAnnotationsValidation = true;
            });

            return services;
        }

        private static IServiceCollection AddApplicationMapping(this IServiceCollection services)
        {
            TypeAdapterConfig.GlobalSettings.Scan(typeof(OrderMappingConfig).Assembly);
            return services;
        }
    }
}

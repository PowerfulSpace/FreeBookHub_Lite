using FluentValidation;
using FluentValidation.AspNetCore;
using Mapster;
using Microsoft.Extensions.DependencyInjection;
using PS.PaymentService.Application.Mappings;

namespace PS.PaymentService.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services
                .AddApplicationMediatR()
                .AddApplicationValidation()
                .AddApplicationMapping();

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
            TypeAdapterConfig.GlobalSettings.Scan(typeof(PaymentMappingConfig).Assembly);
            return services;
        }
    }
}

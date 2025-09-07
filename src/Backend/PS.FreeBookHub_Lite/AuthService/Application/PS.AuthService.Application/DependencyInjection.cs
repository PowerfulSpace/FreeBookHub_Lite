using FluentValidation;
using FluentValidation.AspNetCore;
using Mapster;
using Microsoft.Extensions.DependencyInjection;
using PS.AuthService.Application.Mappings;

namespace PS.AuthService.Application
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
            //services.AddValidatorsFromAssemblyContaining<LoginRequestValidator>();
            //services.AddValidatorsFromAssemblyContaining<RefreshTokenRequestValidator>();
            //services.AddValidatorsFromAssemblyContaining<RegisterUserRequestValidator>();
            //services.AddValidatorsFromAssemblyContaining<LogoutRequestValidator>();

            services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

            services.AddFluentValidationAutoValidation(options =>
            {
                options.DisableDataAnnotationsValidation = true;
            });

            return services;
        }

        private static IServiceCollection AddApplicationMapping(this IServiceCollection services)
        {
            TypeAdapterConfig.GlobalSettings.Scan(typeof(AuthMappingConfig).Assembly);
            return services;
        }
    }
}

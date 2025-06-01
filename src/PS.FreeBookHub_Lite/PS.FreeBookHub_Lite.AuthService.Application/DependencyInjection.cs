using FluentValidation;
using FluentValidation.AspNetCore;
using Mapster;
using Microsoft.Extensions.DependencyInjection;
using PS.FreeBookHub_Lite.AuthService.Application.Mappings;
using PS.FreeBookHub_Lite.AuthService.Application.Services;
using PS.FreeBookHub_Lite.AuthService.Application.Services.Interfaces;
using PS.FreeBookHub_Lite.AuthService.Application.Validators;

namespace PS.FreeBookHub_Lite.AuthService.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IAuthBookService, AuthBookService>();

            services.AddValidatorsFromAssemblyContaining<LoginRequestValidator>();
            services.AddValidatorsFromAssemblyContaining<RefreshTokenRequestValidator>();
            services.AddValidatorsFromAssemblyContaining<RegisterUserRequestValidator>();
            services.AddValidatorsFromAssemblyContaining<LogoutRequestValidator>();

            services.AddFluentValidationAutoValidation(options =>
            {
                options.DisableDataAnnotationsValidation = true;
            });

            TypeAdapterConfig.GlobalSettings.Scan(typeof(AuthMappingConfig).Assembly);

            return services;
        }
    }
}

using FluentValidation;
using FluentValidation.AspNetCore;
using Mapster;
using Microsoft.Extensions.DependencyInjection;
using PS.FreeBookHub_Lite.OrderService.Application.Mappings;
using PS.FreeBookHub_Lite.OrderService.Application.Services;
using PS.FreeBookHub_Lite.OrderService.Application.Services.Interfaces;
using PS.FreeBookHub_Lite.OrderService.Application.Validators;

namespace PS.FreeBookHub_Lite.OrderService.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IOrderProcessingService, OrderProcessingService>();

            services.AddValidatorsFromAssemblyContaining<CreateOrderItemRequestValidator>();
            services.AddValidatorsFromAssemblyContaining<CreateOrderRequestValidator>();

            services.AddFluentValidationAutoValidation(options =>
            {
                options.DisableDataAnnotationsValidation = true;
            });

            TypeAdapterConfig.GlobalSettings.Scan(typeof(OrderMappingConfig).Assembly);

            return services;
        }
    }
}

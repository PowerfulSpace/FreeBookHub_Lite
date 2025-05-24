using FluentValidation;
using FluentValidation.AspNetCore;
using Mapster;
using Microsoft.Extensions.DependencyInjection;
using PS.FreeBookHub_Lite.PaymentService.Application.Mappings;
using PS.FreeBookHub_Lite.PaymentService.Application.Services;
using PS.FreeBookHub_Lite.PaymentService.Application.Services.Interfaces;
using PS.FreeBookHub_Lite.PaymentService.Application.Validators;

namespace PS.FreeBookHub_Lite.PaymentService.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IPaymentBookService, PaymentBookService>();

            services.AddValidatorsFromAssemblyContaining<CreatePaymentRequestValidator>();

            services.AddFluentValidationAutoValidation(options =>
            {
                options.DisableDataAnnotationsValidation = true;
            });

            TypeAdapterConfig.GlobalSettings.Scan(typeof(PaymentMappingConfig).Assembly);

            return services;
        }
    }
}

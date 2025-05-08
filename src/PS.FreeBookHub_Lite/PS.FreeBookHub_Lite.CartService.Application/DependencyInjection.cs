using FluentValidation;
using FluentValidation.AspNetCore;
using Mapster;
using Microsoft.Extensions.DependencyInjection;
using PS.FreeBookHub_Lite.CartService.Application.Mapping;
using PS.FreeBookHub_Lite.CartService.Application.Services;
using PS.FreeBookHub_Lite.CartService.Application.Services.Interfaces;
using PS.FreeBookHub_Lite.CartService.Application.Validators;

namespace PS.FreeBookHub_Lite.CartService.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<ICartBookService, CartBookService>();
            services.AddScoped<IBookCatalogClient, BookCatalogClientStub>();

            services.AddValidatorsFromAssemblyContaining<AddItemRequestValidator>();
            services.AddValidatorsFromAssemblyContaining<UpdateItemQuantityRequestValidator>();

            //Отключает встроенную валидацию DataAnnotations
            //Оставляет только FluentValidation (чтобы не было дублирования)
            services.AddFluentValidationAutoValidation(options =>
            {
                options.DisableDataAnnotationsValidation = true;
            });

            // Регистрация всех маппингов, реализующих IRegister
            TypeAdapterConfig.GlobalSettings.Scan(typeof(CartMappingConfig).Assembly);

            return services;
        }
    }
}

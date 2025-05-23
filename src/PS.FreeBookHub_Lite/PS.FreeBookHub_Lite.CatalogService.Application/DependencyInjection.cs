using FluentValidation;
using FluentValidation.AspNetCore;
using Mapster;
using Microsoft.Extensions.DependencyInjection;
using PS.FreeBookHub_Lite.CatalogService.Application.Mapping;
using PS.FreeBookHub_Lite.CatalogService.Application.Services;
using PS.FreeBookHub_Lite.CatalogService.Application.Services.Interfaces;
using PS.FreeBookHub_Lite.CatalogService.Application.Validators;

namespace PS.FreeBookHub_Lite.CatalogService.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IBookService, BookService>();

            services.AddValidatorsFromAssemblyContaining<CreateBookRequestValidator>();
            services.AddValidatorsFromAssemblyContaining<UpdateBookRequestValidator>();

            //Отключает встроенную валидацию DataAnnotations
            //Оставляет только FluentValidation (чтобы не было дублирования)
            services.AddFluentValidationAutoValidation(options =>
            {
                options.DisableDataAnnotationsValidation = true;
            });

            // Регистрация всех маппингов, реализующих IRegister
            TypeAdapterConfig.GlobalSettings.Scan(typeof(BookMappingConfig).Assembly);

            return services;
        }
    }
}

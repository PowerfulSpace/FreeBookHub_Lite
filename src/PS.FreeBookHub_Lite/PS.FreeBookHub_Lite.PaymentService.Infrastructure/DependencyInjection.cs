using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PS.FreeBookHub_Lite.PaymentService.Application.Interfaces;
using PS.FreeBookHub_Lite.PaymentService.Common.Events.Interfaces;
using PS.FreeBookHub_Lite.PaymentService.Infrastructure.Messaging;
using PS.FreeBookHub_Lite.PaymentService.Infrastructure.Messaging.Consumers;
using PS.FreeBookHub_Lite.PaymentService.Infrastructure.Persistence;
using PS.FreeBookHub_Lite.PaymentService.Infrastructure.Persistence.Repositories;

namespace PS.FreeBookHub_Lite.PaymentService.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, ConfigurationManager configuration)
        {
            services
                .AddPersistance(configuration);

            services.AddSingleton<IEventPublisher, RabbitMqEventPublisher>();
            services.AddHostedService<OrderCreatedConsumer>();

            return services;
        }

        private static IServiceCollection AddPersistance(this IServiceCollection services, ConfigurationManager configuration)
        {
            services.AddDbContext<PaymentDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("PaymentDb")));

            services.AddScoped<IPaymentRepository, PaymentRepository>();

            return services;
        }
    }
}

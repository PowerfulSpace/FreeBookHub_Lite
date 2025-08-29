using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PS.FreeBookHub_Lite.PaymentService.Application.Interfaces;
using PS.FreeBookHub_Lite.PaymentService.Application.Interfaces.Redis;
using PS.FreeBookHub_Lite.PaymentService.Common.Configuration;
using PS.FreeBookHub_Lite.PaymentService.Common.Events.Interfaces;
using PS.FreeBookHub_Lite.PaymentService.Infrastructure.Caching.Redis;
using PS.FreeBookHub_Lite.PaymentService.Infrastructure.Messaging;
using PS.FreeBookHub_Lite.PaymentService.Infrastructure.Messaging.Consumers;
using PS.FreeBookHub_Lite.PaymentService.Infrastructure.Persistence;
using PS.FreeBookHub_Lite.PaymentService.Infrastructure.Persistence.Repositories;
using StackExchange.Redis;

namespace PS.FreeBookHub_Lite.PaymentService.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, ConfigurationManager configuration)
        {
            services
                .AddPersistance(configuration)
                .AddRabbitMqIntegration(configuration)
                .AddRedis(configuration);

            return services;
        }

        private static IServiceCollection AddPersistance(this IServiceCollection services, ConfigurationManager configuration)
        {
            services.AddDbContext<PaymentDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("PaymentDb")));

            services.AddScoped<IPaymentRepository, PaymentRepository>();

            return services;
        }

        private static IServiceCollection AddRabbitMqIntegration(this IServiceCollection services, ConfigurationManager configuration)
        {
            services.Configure<RabbitMqConfig>(configuration.GetSection("RabbitMQ"));

            services.AddSingleton<IEventPublisher, RabbitMqEventPublisher>();
            services.AddHostedService<OrderCreatedConsumer>();
            services.AddHostedService<OrderCreatedDlqConsumer>();

            return services;
        }

        private static IServiceCollection AddRedis(this IServiceCollection services, ConfigurationManager configuration)
        {
            var redisConnection = configuration["Redis:ConnectionString"];

            services.AddSingleton<IConnectionMultiplexer>(
                _ => ConnectionMultiplexer.Connect(redisConnection!));

            services.AddScoped<IEventDeduplicationService, RedisEventDeduplicationService>();

            return services;
        }
    }
}

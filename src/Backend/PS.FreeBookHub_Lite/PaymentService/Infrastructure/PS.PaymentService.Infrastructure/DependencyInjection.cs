using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PS.PaymentService.Application.Interfaces;
using PS.PaymentService.Application.Interfaces.Redis;
using PS.PaymentService.Common.Configuration;
using PS.PaymentService.Common.Events.Interfaces;
using PS.PaymentService.Infrastructure.Caching.Redis;
using PS.PaymentService.Infrastructure.Messaging;
using PS.PaymentService.Infrastructure.Messaging.Consumers;
using PS.PaymentService.Infrastructure.Persistence;
using PS.PaymentService.Infrastructure.Persistence.Repositories;
using StackExchange.Redis;

namespace PS.PaymentService.Infrastructure
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

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PS.FreeBookHub_Lite.OrderService.Application.Clients;
using PS.FreeBookHub_Lite.OrderService.Application.Interfaces;
using PS.FreeBookHub_Lite.OrderService.Application.Interfaces.Redis;
using PS.FreeBookHub_Lite.OrderService.Application.Security;
using PS.FreeBookHub_Lite.OrderService.Common.Configuration;
using PS.FreeBookHub_Lite.OrderService.Common.Events.Interfaces;
using PS.FreeBookHub_Lite.OrderService.Infrastructure.Caching.Redis;
using PS.FreeBookHub_Lite.OrderService.Infrastructure.Clients;
using PS.FreeBookHub_Lite.OrderService.Infrastructure.Http.Handlers;
using PS.FreeBookHub_Lite.OrderService.Infrastructure.Messaging;
using PS.FreeBookHub_Lite.OrderService.Infrastructure.Messaging.Consumers;
using PS.FreeBookHub_Lite.OrderService.Infrastructure.Persistence;
using PS.FreeBookHub_Lite.OrderService.Infrastructure.Persistence.Repositories;
using PS.FreeBookHub_Lite.OrderService.Infrastructure.Security;
using StackExchange.Redis;

namespace PS.FreeBookHub_Lite.OrderService.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, ConfigurationManager configuration)
        {
            services
                .AddPersistance(configuration)
                .AddHttpClients(configuration)
                .AddRabbitMqIntegration(configuration)
                .AddRedis(configuration);

            services.AddHttpContextAccessor();

            services.AddScoped<IAccessTokenProvider, HttpContextAccessTokenProvider>();

            services.AddTransient<AccessTokenHandler>();
            services.AddTransient<InternalAuthHandler>();

            return services;
        }

        private static IServiceCollection AddPersistance(this IServiceCollection services, ConfigurationManager configuration)
        {
            services.AddDbContext<OrderDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("OrderDb")));

            services.AddScoped<IOrderRepository, OrderRepository>();

            return services;
        }

        private static IServiceCollection AddHttpClients(this IServiceCollection services, ConfigurationManager configuration)
        {
            services.AddHttpClient<IPaymentServiceClient, PaymentServiceClient>(client =>
            {
                client.BaseAddress = new Uri(configuration["PaymentService:BaseUrl"] ?? "https://localhost:7177");
            })
            .AddHttpMessageHandler<AccessTokenHandler>()
            .AddHttpMessageHandler<InternalAuthHandler>();

            return services;
        }

        private static IServiceCollection AddRabbitMqIntegration(this IServiceCollection services, ConfigurationManager configuration)
        {
            services.Configure<RabbitMqConfig>(configuration.GetSection("RabbitMQ"));

            services.AddSingleton<IEventPublisher, RabbitMqEventPublisher>();
            services.AddHostedService<PaymentCompletedConsumer>();
            services.AddHostedService<PaymentCompletedDlqConsumer>();

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

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PS.OrderService.Common.Configuration;
using PS.OrderService.Infrastructure.Messaging;
using RabbitMQ.Client;
using System.Reflection;

namespace PS.OrderService.UnitTests.Infrastructure.Messaging
{
    public class RabbitMqEventPublisherTests
    {

        
    }


    internal class TestableRabbitMqEventPublisher : RabbitMqEventPublisher
    {
        public TestableRabbitMqEventPublisher(
            ILogger<RabbitMqEventPublisher> logger,
            IOptions<RabbitMqConfig> config,
            IConnection connection,
            IModel channel)
            : base(logger, config)
        {
            typeof(RabbitMqEventPublisher)
                .GetField("_connection", BindingFlags.NonPublic | BindingFlags.Instance)!
                .SetValue(this, connection);

            typeof(RabbitMqEventPublisher)
                .GetField("_channel", BindingFlags.NonPublic | BindingFlags.Instance)!
                .SetValue(this, channel);
        }
    }
}

namespace PS.FreeBookHub_Lite.PaymentService.Common.Configuration
{
    public class RabbitMqConfig
    {
        public string HostName { get; set; } = "localhost";
        public string ExchangeName { get; set; } = "bookhub.exchange";

        public QueuesConfig Queues { get; set; } = new();
        public RoutingKeysConfig RoutingKeys { get; set; } = new();
        public DeadLetterConfig DeadLetter { get; set; } = new();
        public RetryPolicyConfig RetryPolicy { get; set; } = new();

        public class QueuesConfig
        {
            public string OrderCreatedQueue { get; set; } = "order.created";
        }

        public class RoutingKeysConfig
        {
            public string OrderCreatedRoutingKey { get; set; } = "order.created";
            public string PaymentCompletedRoutingKey { get; set; } = "payment.completed";
        }

        public class DeadLetterConfig
        {
            public string Exchange { get; set; } = "bookhub.exchange.dlx";
            public string Queue { get; set; } = "order.created.dlq";
            public string RoutingKey { get; set; } = "order.created.dlq";
        }

        public class RetryPolicyConfig
        {
            public int IntervalMs { get; set; } = 5000;
            public int MaxRetryCount { get; set; } = 5;
        }
    }
}

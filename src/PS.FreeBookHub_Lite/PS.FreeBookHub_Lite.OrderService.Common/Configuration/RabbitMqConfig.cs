namespace PS.FreeBookHub_Lite.OrderService.Common.Configuration
{
    public class RabbitMqConfig
    {
        public string HostName { get; set; } = "localhost";
        public string ExchangeName { get; set; } = "bookhub.exchange";
        public string PaymentCompletedQueue { get; set; } = "payment.completed";
        public string PaymentCompletedRoutingKey { get; set; } = "payment.completed";
        public string OrderCreatedRoutingKey { get; set; } = "order.created";

        public string PaymentCompletedDeadLetterExchange { get; set; } = "bookhub.exchange.dlx";
        public string PaymentCompletedDeadLetterQueue { get; set; } = "payment.completed.dlq";
        public string PaymentCompletedDeadLetterRoutingKey { get; set; } = "payment.completed.dlq";

        public int RetryIntervalMs { get; set; } = 5000;
        public int MaxRetryCount { get; set; } = 5;
    }

    //public class RabbitMqConfig
    //{
    //    public string HostName { get; set; } = "localhost";
    //    public string ExchangeName { get; set; } = "bookhub.exchange";

    //    public RoutingKeysConfig RoutingKeys { get; set; } = new();
    //    public DeadLetterConfig DeadLetter { get; set; } = new();
    //    public RetryPolicyConfig RetryPolicy { get; set; } = new();

    //    public class RoutingKeysConfig
    //    {
    //        public string PaymentCompletedQueue { get; set; } = "payment.completed";
    //        public string PaymentCompletedRoutingKey { get; set; } = "payment.completed";
    //        public string OrderCreatedRoutingKey { get; set; } = "order.created";
    //    }

    //    public class DeadLetterConfig
    //    {
    //        public string Exchange { get; set; } = "bookhub.exchange.dlx";
    //        public string Queue { get; set; } = "payment.completed.dlq";
    //        public string RoutingKey { get; set; } = "payment.completed.dlq";
    //    }

    //    public class RetryPolicyConfig
    //    {
    //        public int IntervalMs { get; set; } = 5000;
    //        public int MaxRetryCount { get; set; } = 5;
    //    }
    //}
}

namespace PS.FreeBookHub_Lite.OrderService.Common.Options
{
    public class RabbitMqConfig
    {
        public string HostName { get; set; } = "localhost";
        public string ExchangeName { get; set; } = "bookhub.exchange";
        public string PaymentCompletedQueue { get; set; } = "payment.completed";
        public string PaymentCompletedRoutingKey { get; set; } = "payment.completed";
        public string OrderCreatedRoutingKey { get; set; } = "order.created";
    }
}

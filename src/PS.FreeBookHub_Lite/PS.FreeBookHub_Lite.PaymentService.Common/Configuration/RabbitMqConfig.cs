namespace PS.FreeBookHub_Lite.PaymentService.Common.Configuration
{
    public class RabbitMqConfig
    {
        public string HostName { get; set; } = "localhost";
        public string ExchangeName { get; set; } = "bookhub.exchange";
        public string OrderCreatedQueue { get; set; } = "order.created";
        public string OrderCreatedRoutingKey { get; set; } = "order.created";
        public string PaymentCompletedRoutingKey { get; set; } = "payment.completed";
    }
}

namespace PS.FreeBookHub_Lite.OrderService.Common.Logging
{
    public static class LoggerMessages
    {
        // ExceptionHandlingMiddleware
        //                  --- Order Errors
        public const string OrderNotFound = "Order not found — OrderId: {OrderId} | Method: {Method} | Path: {Path}";
        public const string PaymentFailedLog = "Payment failed — OrderId: {OrderId} | Method: {Method} | Path: {Path}";
        public const string InvalidOrderOperation = "Invalid order operation — Message: {Message} | Method: {Method} | Path: {Path}";
        public const string InvalidUserIdentifier = "Invalid user identifier — InvalidId: {InvalidId} | Method: {Method} | Path: {Path}";

        //                  --- General Error Handling
        public const string UnhandledException = "Unhandled exception — Message: {Message} | Method: {Method} | Path: {Path}";

        // Business Logic Errors
        public const string CannotCancelOrder = "Cannot cancel order - OrderId: {OrderId}, CurrentStatus: {Status} | Method: {Method} | Path: {Path}";
        public const string InvalidPaymentState = "Invalid payment state - OrderId: {OrderId}, CurrentStatus: {Status} | Method: {Method} | Path: {Path}";
        public const string InvalidQuantity = "Invalid quantity - Quantity: {Quantity} | Method: {Method} | Path: {Path}";


        // OrderBookService
        //                  --- CreateOrderAsync
        public const string CreateOrderStarted = "[ORDER] CREATE started | UserId:{UserId}";
        public const string CreateOrderSuccess = "[ORDER] CREATE success | OrderId:{OrderId} | UserId:{UserId}";

        //                  --- GetAllOrdersByUserIdAsync
        public const string GetAllOrdersStarted = "[ORDER] GET_ALL started | UserId:{UserId}";
        public const string GetAllOrdersSuccess = "[ORDER] GET_ALL success | Count:{Count} | UserId:{UserId}";

        //                  --- GetOrderByIdAsync
        public const string GetOrderByIdStarted = "[ORDER] GET_BY_ID started | OrderId:{OrderId}";
        public const string GetOrderByIdSuccess = "[ORDER] GET_BY_ID success | OrderId:{OrderId}";

        //                  --- CancelOrderAsync
        public const string CancelOrderStarted = "[ORDER] CANCEL started | OrderId:{OrderId}";
        public const string CancelOrderSuccess = "[ORDER] CANCEL success | OrderId:{OrderId}";

        // PaymentServiceClient
        public const string PaymentCreationStarted = "[PAYMENT] CREATE started | OrderId:{OrderId}";
        public const string PaymentCreationSuccess = "[PAYMENT] CREATE success | OrderId:{OrderId}";


        // RabbitMQ Consumer Logs
        //                  --- PaymentCompletedConsumer
        public const string PaymentConsumerStarted = "[RABBITMQ] PAYMENT_CONSUMER started listening queue: {Queue}";
        public const string PaymentConsumerStopped = "[RABBITMQ] PAYMENT_CONSUMER stopped listening queue: {Queue}";
        public const string PaymentMessageReceived = "[RABBITMQ] PAYMENT_MESSAGE received | OrderId:{OrderId} | PaymentId:{PaymentId}";
        public const string PaymentMessageProcessing = "[RABBITMQ] PAYMENT_MESSAGE processing | OrderId:{OrderId}";
        public const string PaymentOrderNotFound = "[RABBITMQ] PAYMENT_ORDER not found | OrderId:{OrderId}";
        public const string PaymentOrderMarkedAsPaid = "[RABBITMQ] PAYMENT_ORDER marked as paid | OrderId:{OrderId}";
        public const string PaymentMessageProcessed = "[RABBITMQ] PAYMENT_MESSAGE processed | OrderId:{OrderId} | Duration:{ElapsedMs}ms";
        public const string PaymentMessageDeserializeError = "[RABBITMQ] PAYMENT_MESSAGE deserialize error | Body:{MessageBody}";
        public const string PaymentProcessingError = "[RABBITMQ] PAYMENT_PROCESSING error | OrderId:{OrderId} | Error:{ErrorMessage}";
        public const string PaymentDuplicateDetected = "[RABBITMQ] PAYMENT_DUPLICATE detected | MessageId:{MessageId} | PaymentId:{PaymentId}";
        public const string PaymentRetryLimitExceeded = "[RABBITMQ] PAYMENT_RETRY_LIMIT exceeded, message will be dead-lettered | DeliveryTag:{DeliveryTag} | MessageId:{MessageId}";

        //                  --- PaymentCompletedDlqConsumer
        public const string DlqConsumerStarted = "[RABBITMQ] DLQ_CONSUMER started | Queue:{Queue}";
        public const string DlqConsumerStopped = "[RABBITMQ] DLQ_CONSUMER stopped | Queue:{Queue}";
        public const string DlqMessageReceived = "[RABBITMQ] DLQ_MESSAGE received | Body:{MessageBody}";

        //                  --- EventPublisher
        public const string EventPublisherCreated = "[RABBITMQ] EVENT_PUBLISHER created | Exchange:{Exchange}";
        public const string OrderEventPublished = "[RABBITMQ] ORDER_EVENT published | Type:{EventType} | RoutingKey:{RoutingKey}";
        public const string OrderEventPublishError = "[RABBITMQ] ORDER_EVENT publish error | Type:{EventType} | Error:{ErrorMessage}";
    }
}

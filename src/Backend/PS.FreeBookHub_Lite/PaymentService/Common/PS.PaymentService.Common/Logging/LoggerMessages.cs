namespace PS.FreeBookHub_Lite.PaymentService.Common.Logging
{
    public static class LoggerMessages
    {
        // ExceptionHandlingMiddleware
        //                  --- Payment Errors
        public const string PaymentNotFound = "Payment not found — PaymentId: {PaymentId} | Method: {Method} | Path: {Path}";
        public const string UnauthorizedAccess = "Unauthorized access — PaymentId: {PaymentId}, UserId: {UserId} | Method: {Method} | Path: {Path}";
        public const string DuplicatePayment = "Duplicate payment — OrderId: {OrderId} | Method: {Method} | Path: {Path}";
        public const string InvalidPaymentStatus = "Invalid payment status — Current: {CurrentStatus}, Required: {RequiredStatus} | Method: {Method} | Path: {Path}";
        public const string InvalidUserIdentifier = "Invalid user identifier — InvalidId: {InvalidId} | Method: {Method} | Path: {Path}";

        //                  --- General Error Handling
        public const string UnhandledException = "Unhandled exception — Message: {Message} | Method: {Method} | Path: {Path}";


        // PaymentBookService
        //                  --- ProcessPaymentAsync
        public const string ProcessPaymentStarted = "[PAYMENT] PROCESS started | OrderId:{OrderId} | UserId:{UserId}";
        public const string ProcessPaymentSuccess = "[PAYMENT] PROCESS success | PaymentId:{PaymentId} | OrderId:{OrderId}";

        //                  --- GetPaymentByIdAsync
        public const string GetPaymentByIdStarted = "[PAYMENT] GET_BY_ID started | PaymentId:{PaymentId}";
        public const string GetPaymentByIdSuccess = "[PAYMENT] GET_BY_ID success | PaymentId:{PaymentId}";

        //                  --- GetPaymentsByOrderIdAsync
        public const string GetPaymentsByOrderStarted = "[PAYMENT] GET_BY_ORDER started | OrderId:{OrderId}";
        public const string GetPaymentsByOrderSuccess = "[PAYMENT] GET_BY_ORDER success | OrderId:{OrderId} | Count:{Count}";


        // RabbitMQ Consumer Logs
        //                  --- OrderCreatedConsumer
        public const string OrderConsumerStarted = "[RABBITMQ] ORDER_CONSUMER started listening queue: {Queue}";
        public const string OrderConsumerStopped = "[RABBITMQ] ORDER_CONSUMER stopped listening queue: {Queue}";
        public const string OrderMessageReceived = "[RABBITMQ] ORDER_MESSAGE received | OrderId:{OrderId} | UserId:{UserId} | Amount:{Amount}";
        public const string OrderProcessingStarted = "[RABBITMQ] ORDER_PROCESSING started | OrderId:{OrderId}";
        public const string PaymentProcessingStarted = "[RABBITMQ] PAYMENT_PROCESSING started | OrderId:{OrderId}";
        public const string PaymentProcessed = "[RABBITMQ] PAYMENT_PROCESSED | PaymentId:{PaymentId} | OrderId:{OrderId}";
        public const string PaymentEventPublished = "[RABBITMQ] PAYMENT_EVENT published | OrderId:{OrderId} | PaymentId:{PaymentId}";
        public const string OrderMessageProcessed = "[RABBITMQ] ORDER_MESSAGE processed | OrderId:{OrderId} | Duration:{ElapsedMs}ms";
        public const string OrderMessageDeserializeError = "[RABBITMQ] ORDER_MESSAGE deserialize error | Body:{MessageBody}";
        public const string OrderProcessingError = "[RABBITMQ] ORDER_PROCESSING error | OrderId:{OrderId} | Error:{ErrorMessage}";
        public const string OrderMessageDuplicate = "[RABBITMQ] ORDER_MESSAGE duplicate detected | MessageId:{MessageId} | OrderId:{OrderId}";
        public const string OrderMessageRetryLimitExceeded = "[RABBITMQ] ORDER_MESSAGE retry limit exceeded — DeliveryTag:{DeliveryTag} | MessageId:{MessageId}";

        //                  --- OrderCreatedDlqConsumer
        public const string DlqConsumerInitialized = "[RABBITMQ] DLQ_CONSUMER initialized for queue: {Queue}";
        public const string DlqMessageReceived = "[RABBITMQ] DLQ_MESSAGE received from OrderCreated DLQ | Message:{Message}";
        public const string DlqConsumerStopped = "[RABBITMQ] DLQ_CONSUMER stopped for queue: {Queue}";

        //                  --- EventPublisher
        public const string EventPublisherCreated = "[RABBITMQ] EVENT_PUBLISHER created | Exchange:{Exchange}";
        public const string EventPublished = "[RABBITMQ] EVENT published | Type:{EventType} | RoutingKey:{RoutingKey}";
        public const string EventPublishError = "[RABBITMQ] EVENT publish error | Type:{EventType} | Error:{ErrorMessage}";
    }

}
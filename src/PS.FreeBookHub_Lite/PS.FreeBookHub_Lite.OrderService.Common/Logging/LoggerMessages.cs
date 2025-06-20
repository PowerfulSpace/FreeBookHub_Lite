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
    }
}

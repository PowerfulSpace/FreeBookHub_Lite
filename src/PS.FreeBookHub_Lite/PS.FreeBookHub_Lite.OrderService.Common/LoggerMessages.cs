namespace PS.FreeBookHub_Lite.OrderService.Common
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
        public const string CreateOrderStarted = "Creating order — UserId: {UserId}";
        public const string CreateOrderSuccess = "Order created — OrderId: {OrderId}, UserId: {UserId}";

        //                  --- GetAllOrdersByUserIdAsync
        public const string GetAllOrdersStarted = "Fetching orders — UserId: {UserId}";
        public const string GetAllOrdersSuccess = "Orders retrieved — Count: {Count}, UserId: {UserId}";

        //                  --- GetOrderByIdAsync
        public const string GetOrderByIdStarted = "Fetching order — OrderId: {OrderId}";
        public const string GetOrderByIdSuccess = "Order retrieved — OrderId: {OrderId}";

        //                  --- CancelOrderAsync
        public const string CancelOrderStarted = "Cancelling order — OrderId: {OrderId}";
        public const string CancelOrderSuccess = "Order cancelled — OrderId: {OrderId}";


        // PaymentServiceClient
        public const string PaymentCreationStarted = "Creating payment for order {OrderId}";
        public const string PaymentCreationSuccess = "Payment created successfully for order {OrderId}";
    }
}

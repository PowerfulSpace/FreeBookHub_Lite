namespace PS.FreeBookHub_Lite.PaymentService.Common
{
    public static class LoggerMessages
    {
        // Payment Errors
        public const string PaymentNotFound = "Payment not found — PaymentId: {PaymentId} | Method: {Method} | Path: {Path}";
        public const string UnauthorizedAccess = "Unauthorized access — PaymentId: {PaymentId}, UserId: {UserId} | Method: {Method} | Path: {Path}";
        public const string DuplicatePayment = "Duplicate payment — OrderId: {OrderId} | Method: {Method} | Path: {Path}";
        public const string InvalidPaymentStatus = "Invalid payment status — Current: {CurrentStatus}, Required: {RequiredStatus} | Method: {Method} | Path: {Path}";
        public const string InvalidUserIdentifier = "Invalid user identifier — InvalidId: {InvalidId} | Method: {Method} | Path: {Path}";

        // General Error
        public const string UnhandledException = "Unhandled exception — Message: {Message} | Method: {Method} | Path: {Path}";

        // ProcessPaymentAsync
        public const string ProcessPaymentStarted = "Processing payment — OrderId: {OrderId}, UserId: {UserId}";
        public const string ProcessPaymentSuccess = "Payment processed — PaymentId: {PaymentId}, OrderId: {OrderId}";

        // GetPaymentByIdAsync
        public const string GetPaymentByIdStarted = "Fetching payment — PaymentId: {PaymentId}";
        public const string GetPaymentByIdSuccess = "Payment retrieved — PaymentId: {PaymentId}";

        // GetPaymentsByOrderIdAsync
        public const string GetPaymentsByOrderStarted = "Fetching payments — OrderId: {OrderId}";
        public const string GetPaymentsByOrderSuccess = "Payments retrieved — OrderId: {OrderId}, Count: {Count}";

    }

}

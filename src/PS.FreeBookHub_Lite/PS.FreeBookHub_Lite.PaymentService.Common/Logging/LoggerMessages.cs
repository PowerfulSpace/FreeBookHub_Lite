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
    }

}
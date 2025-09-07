using PS.OrderService.Domain.Exceptions.Base;

namespace PS.OrderService.Domain.Exceptions.Payment
{
    public class PaymentFailedException : OrderServiceException
    {
        public Guid OrderId { get; }
        public int StatusCode { get; }
        public string ErrorResponse { get; }

        public PaymentFailedException(Guid orderId, int statusCode, string errorResponse)
            : base($"Payment failed for order {orderId}. Status: {statusCode}")
        {
            OrderId = orderId;
            StatusCode = statusCode;
            ErrorResponse = errorResponse;
        }
    }
}

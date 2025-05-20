namespace PS.FreeBookHub_Lite.OrderService.Domain.Exceptions
{
    public class PaymentFailedException :Exception
    {
        public PaymentFailedException(Guid orderId) 
            : base($"Payment for order {orderId} failed") { }

    }
}

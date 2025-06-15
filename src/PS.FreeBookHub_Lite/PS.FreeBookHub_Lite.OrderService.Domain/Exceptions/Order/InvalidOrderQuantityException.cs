using PS.FreeBookHub_Lite.OrderService.Domain.Exceptions.Base;

namespace PS.FreeBookHub_Lite.OrderService.Domain.Exceptions.Order
{
    public class InvalidOrderQuantityException : OrderServiceException
    {
        public int ProvidedQuantity { get; }

        public InvalidOrderQuantityException(int quantity)
            : base($"Invalid quantity: {quantity}. Quantity must be greater than zero.")
        {
            ProvidedQuantity = quantity;
        }
    }
}

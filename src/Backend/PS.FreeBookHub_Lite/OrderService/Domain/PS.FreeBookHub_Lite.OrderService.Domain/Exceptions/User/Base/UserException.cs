using PS.FreeBookHub_Lite.OrderService.Domain.Exceptions.Base;

namespace PS.FreeBookHub_Lite.OrderService.Domain.Exceptions.User.Base
{
    public abstract class UserException : OrderServiceException
    {
        protected UserException(string message) : base(message) { }
    }
}

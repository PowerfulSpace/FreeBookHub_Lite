using PS.OrderService.Domain.Exceptions.Base;

namespace PS.OrderService.Domain.Exceptions.User.Base
{
    public abstract class UserException : OrderServiceException
    {
        protected UserException(string message) : base(message) { }
    }
}

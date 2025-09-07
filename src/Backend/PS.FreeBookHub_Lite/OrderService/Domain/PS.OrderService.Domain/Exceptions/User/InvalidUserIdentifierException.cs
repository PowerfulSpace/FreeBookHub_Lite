using PS.OrderService.Domain.Exceptions.User.Base;

namespace PS.OrderService.Domain.Exceptions.User
{
    public class InvalidUserIdentifierException : UserException
    {
        public string InvalidId { get; }

        public InvalidUserIdentifierException(string invalidId)
            : base($"Invalid user ID format: '{invalidId}'")
        {
            InvalidId = invalidId;
        }
    }
}

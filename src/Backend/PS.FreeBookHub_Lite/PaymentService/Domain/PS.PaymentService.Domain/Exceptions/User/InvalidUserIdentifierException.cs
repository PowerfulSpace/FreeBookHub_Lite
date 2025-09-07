using PS.PaymentService.Domain.Exceptions.User.Base;

namespace PS.PaymentService.Domain.Exceptions.User
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

using PS.FreeBookHub_Lite.AuthService.Domain.Exceptions.Base;

namespace PS.FreeBookHub_Lite.AuthService.Domain.Exceptions.User
{
    public class InvalidUserIdentifierException : AuthServiceException
    {
        public string InvalidId { get; }

        public InvalidUserIdentifierException(string invalidId)
            : base($"Invalid user ID format: '{invalidId}'")
        {
            InvalidId = invalidId;
        }
    }
}

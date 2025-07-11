﻿using PS.FreeBookHub_Lite.CartService.Domain.Exceptions.User.Base;

namespace PS.FreeBookHub_Lite.CartService.Domain.Exceptions.User
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

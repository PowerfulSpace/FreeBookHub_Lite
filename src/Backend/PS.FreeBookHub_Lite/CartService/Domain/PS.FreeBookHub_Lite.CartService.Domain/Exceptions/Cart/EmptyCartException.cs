﻿using PS.FreeBookHub_Lite.CartService.Domain.Exceptions.Cart.Base;

namespace PS.FreeBookHub_Lite.CartService.Domain.Exceptions.Cart
{
    public class EmptyCartException : CartServiceException
    {
        public Guid UserId { get; }

        public EmptyCartException(Guid userId)
            : base($"Unable to proceed with checkout: user {userId}'s cart is empty")
        {
            UserId = userId;
        }
    }
}

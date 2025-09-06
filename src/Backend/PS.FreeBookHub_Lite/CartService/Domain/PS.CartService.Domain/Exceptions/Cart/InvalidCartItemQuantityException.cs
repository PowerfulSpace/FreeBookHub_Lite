﻿using PS.FreeBookHub_Lite.CartService.Domain.Exceptions.Cart.Base;

namespace PS.FreeBookHub_Lite.CartService.Domain.Exceptions.Cart
{

    public class InvalidCartItemQuantityException : CartServiceException
    {
        public int Quantity { get; }

        public InvalidCartItemQuantityException(int quantity)
            : base($"Invalid quantity: {quantity}. Quantity must be greater than zero.")
        {
            Quantity = quantity;
        }
    }
}

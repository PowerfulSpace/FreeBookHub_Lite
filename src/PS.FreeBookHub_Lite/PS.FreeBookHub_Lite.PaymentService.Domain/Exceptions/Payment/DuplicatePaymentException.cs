﻿using PS.FreeBookHub_Lite.PaymentService.Domain.Exceptions.Base;

namespace PS.FreeBookHub_Lite.PaymentService.Domain.Exceptions.Payment
{
    public class DuplicatePaymentException : PaymentServiceException
    {
        public Guid OrderId { get; }

        public DuplicatePaymentException(Guid orderId)
            : base($"Order {orderId} already has a completed payment.")
        {
            OrderId = orderId;
        }
    }
}

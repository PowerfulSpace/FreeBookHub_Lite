﻿namespace PS.FreeBookHub_Lite.PaymentService.Application.DTOs
{
    public class CreatePaymentRequest
    {
        public Guid OrderId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
    }
}

namespace PS.PaymentService.Application.DTOs
{
    public class CreatePaymentRequest
    {
        public Guid OrderId { get; set; }
        public Guid UserId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
    }
}

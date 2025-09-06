using MediatR;
using PS.FreeBookHub_Lite.PaymentService.Application.DTOs;

namespace PS.FreeBookHub_Lite.PaymentService.Application.CQRS.Commands.ProcessPayment
{
    public class ProcessPaymentCommand : IRequest<PaymentResponse>
    {
        public Guid OrderId { get; set; }
        public Guid UserId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; }

        public ProcessPaymentCommand(Guid orderId, Guid userId, decimal amount, string paymentMethod)
        {
            OrderId = orderId;
            UserId = userId;
            Amount = amount;
            PaymentMethod = paymentMethod;
        }
    }
}

using MediatR;
using PS.FreeBookHub_Lite.PaymentService.Application.DTOs;

namespace PS.FreeBookHub_Lite.PaymentService.Application.CQRS.Commands.ProcessPayment
{
    public class ProcessPaymentCommand : IRequest<PaymentResponse>
    {
        public Guid OrderId { get; }
        public Guid UserId { get; }
        public decimal Amount { get; }
        public string PaymentMethod { get; }

        public ProcessPaymentCommand(Guid orderId, Guid userId, decimal amount, string paymentMethod)
        {
            OrderId = orderId;
            UserId = userId;
            Amount = amount;
            PaymentMethod = paymentMethod;
        }
    }
}

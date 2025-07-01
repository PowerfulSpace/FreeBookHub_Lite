using MediatR;
using PS.FreeBookHub_Lite.PaymentService.Application.DTOs;

namespace PS.FreeBookHub_Lite.PaymentService.Application.CQRS.Queries.GetPaymentsByOrderId
{
    public class GetPaymentsByOrderIdQuery : IRequest<IEnumerable<PaymentResponse>>
    {
        public Guid OrderId { get; }
        public Guid CurrentUserId { get; }

        public GetPaymentsByOrderIdQuery(Guid orderId, Guid currentUserId)
        {
            OrderId = orderId;
            CurrentUserId = currentUserId;
        }
    }
}

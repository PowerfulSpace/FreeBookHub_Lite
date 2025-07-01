using MediatR;
using PS.FreeBookHub_Lite.PaymentService.Application.DTOs;

namespace PS.FreeBookHub_Lite.PaymentService.Application.CQRS.Queries.GetPaymentById
{
    public class GetPaymentByIdQuery : IRequest<PaymentResponse>
    {
        public Guid PaymentId { get; }
        public Guid CurrentUserId { get; }

        public GetPaymentByIdQuery(Guid paymentId, Guid currentUserId)
        {
            PaymentId = paymentId;
            CurrentUserId = currentUserId;
        }
    }
}

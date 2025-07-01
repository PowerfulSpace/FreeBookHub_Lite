using MediatR;
using PS.FreeBookHub_Lite.OrderService.Application.DTOs;

namespace PS.FreeBookHub_Lite.OrderService.Application.CQRS.Queries.GetOrderById
{
    public class GetOrderByIdQuery : IRequest<OrderResponse>
    {
        public Guid OrderId { get; set; }

        public GetOrderByIdQuery(Guid orderId)
        {
            OrderId = orderId;
        }
    }
}

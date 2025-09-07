using MediatR;
using PS.OrderService.Application.DTOs;

namespace PS.OrderService.Application.CQRS.Queries.GetOrderById
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

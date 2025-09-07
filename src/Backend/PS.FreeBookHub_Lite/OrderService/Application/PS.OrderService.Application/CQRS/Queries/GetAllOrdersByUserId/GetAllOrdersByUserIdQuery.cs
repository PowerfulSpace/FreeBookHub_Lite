using MediatR;
using PS.OrderService.Application.DTOs;

namespace PS.OrderService.Application.CQRS.Queries.GetAllOrdersByUserId
{
    public class GetAllOrdersByUserIdQuery : IRequest<IEnumerable<OrderResponse>>
    {
        public Guid UserId { get; set; }

        public GetAllOrdersByUserIdQuery(Guid userId)
        {
            UserId = userId;
        }
    }
}

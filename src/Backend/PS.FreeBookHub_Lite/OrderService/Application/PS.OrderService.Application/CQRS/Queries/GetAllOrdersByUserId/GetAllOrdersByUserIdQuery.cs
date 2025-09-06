using MediatR;
using PS.FreeBookHub_Lite.OrderService.Application.DTOs;

namespace PS.FreeBookHub_Lite.OrderService.Application.CQRS.Queries.GetAllOrdersByUserId
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

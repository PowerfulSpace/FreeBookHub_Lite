using MediatR;

namespace PS.OrderService.Application.CQRS.Commands.CancelOrder
{
    public class CancelOrderCommand : IRequest<Unit>
    {
        public Guid OrderId { get; set; }

        public CancelOrderCommand(Guid orderId)
        {
            OrderId = orderId;
        }
    }
}

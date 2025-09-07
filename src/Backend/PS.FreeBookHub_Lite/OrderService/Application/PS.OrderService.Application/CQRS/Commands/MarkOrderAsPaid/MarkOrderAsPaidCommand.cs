using MediatR;

namespace PS.OrderService.Application.CQRS.Commands.MarkOrderAsPaid
{
    public class MarkOrderAsPaidCommand : IRequest<Unit>
    {
        public Guid OrderId { get; }

        public MarkOrderAsPaidCommand(Guid orderId)
        {
            OrderId = orderId;
        }
    }
}

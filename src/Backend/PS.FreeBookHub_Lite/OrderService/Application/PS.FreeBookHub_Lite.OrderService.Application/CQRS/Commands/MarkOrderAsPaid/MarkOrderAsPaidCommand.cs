using MediatR;

namespace PS.FreeBookHub_Lite.OrderService.Application.CQRS.Commands.MarkOrderAsPaid
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

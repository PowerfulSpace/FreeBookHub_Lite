using MediatR;
using Microsoft.Extensions.Logging;
using PS.FreeBookHub_Lite.OrderService.Application.Interfaces;
using PS.FreeBookHub_Lite.OrderService.Common.Logging;

namespace PS.FreeBookHub_Lite.OrderService.Application.CQRS.Commands.MarkOrderAsPaid
{
    public class MarkOrderAsPaidCommandHandler : IRequestHandler<MarkOrderAsPaidCommand, Unit>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger<MarkOrderAsPaidCommandHandler> _logger;

        public MarkOrderAsPaidCommandHandler(IOrderRepository orderRepository, ILogger<MarkOrderAsPaidCommandHandler> logger)
        {
            _orderRepository = orderRepository;
            _logger = logger;
        }

        public async Task<Unit> Handle(MarkOrderAsPaidCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation(LoggerMessages.PaymentMessageProcessing, request.OrderId);

            var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);
            if (order is null)
            {
                _logger.LogWarning(LoggerMessages.PaymentOrderNotFound, request.OrderId);
                return Unit.Value;
            }

            order.MarkAsPaid();
            await _orderRepository.UpdateAsync(order, cancellationToken);

            _logger.LogInformation(LoggerMessages.PaymentOrderMarkedAsPaid, request.OrderId);

            return Unit.Value;
        }
    }
}

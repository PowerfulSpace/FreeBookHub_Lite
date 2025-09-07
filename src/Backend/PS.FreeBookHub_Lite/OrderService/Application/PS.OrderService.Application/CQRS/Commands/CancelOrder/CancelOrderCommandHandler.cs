using MediatR;
using Microsoft.Extensions.Logging;
using PS.OrderService.Application.Interfaces;
using PS.OrderService.Common.Logging;
using PS.OrderService.Domain.Exceptions.Order;

namespace PS.OrderService.Application.CQRS.Commands.CancelOrder
{
    public class CancelOrderCommandHandler : IRequestHandler<CancelOrderCommand, Unit>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger<CancelOrderCommandHandler> _logger;

        public CancelOrderCommandHandler(
            IOrderRepository orderRepository,
            ILogger<CancelOrderCommandHandler> logger)
        {
            _orderRepository = orderRepository;
            _logger = logger;
        }

        public async Task<Unit> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation(LoggerMessages.CancelOrderStarted, request.OrderId);

            var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);
            if (order == null)
            {
                throw new OrderNotFoundException(request.OrderId);
            }

            order.Cancel();
            await _orderRepository.UpdateAsync(order, cancellationToken);

            _logger.LogInformation(LoggerMessages.CancelOrderSuccess, request.OrderId);

            return Unit.Value;
        }
    }
}

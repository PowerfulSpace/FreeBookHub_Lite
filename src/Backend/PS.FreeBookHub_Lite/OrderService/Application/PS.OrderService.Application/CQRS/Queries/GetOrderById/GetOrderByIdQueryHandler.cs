using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;
using PS.OrderService.Application.DTOs;
using PS.OrderService.Application.Interfaces;
using PS.OrderService.Common.Logging;
using PS.OrderService.Domain.Exceptions.Order;

namespace PS.OrderService.Application.CQRS.Queries.GetOrderById
{
    public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, OrderResponse>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger<GetOrderByIdQueryHandler> _logger;

        public GetOrderByIdQueryHandler(
            IOrderRepository orderRepository,
            ILogger<GetOrderByIdQueryHandler> logger)
        {
            _orderRepository = orderRepository;
            _logger = logger;
        }

        public async Task<OrderResponse> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation(LoggerMessages.GetOrderByIdStarted, request.OrderId);

            var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken, asNoTracking: true);

            if (order == null)
            {
                throw new OrderNotFoundException(request.OrderId);
            }

            _logger.LogInformation(LoggerMessages.GetOrderByIdSuccess, request.OrderId);

            return order.Adapt<OrderResponse>();
        }
    }
}

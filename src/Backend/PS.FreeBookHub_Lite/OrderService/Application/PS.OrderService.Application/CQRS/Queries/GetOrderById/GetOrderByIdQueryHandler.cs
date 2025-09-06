using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;
using PS.FreeBookHub_Lite.OrderService.Application.DTOs;
using PS.FreeBookHub_Lite.OrderService.Application.Interfaces;
using PS.FreeBookHub_Lite.OrderService.Common.Logging;
using PS.FreeBookHub_Lite.OrderService.Domain.Exceptions.Order;

namespace PS.FreeBookHub_Lite.OrderService.Application.CQRS.Queries.GetOrderById
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

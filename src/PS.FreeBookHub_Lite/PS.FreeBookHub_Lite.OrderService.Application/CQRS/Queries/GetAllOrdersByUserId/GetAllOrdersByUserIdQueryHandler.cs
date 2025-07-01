using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;
using PS.FreeBookHub_Lite.OrderService.Application.DTOs;
using PS.FreeBookHub_Lite.OrderService.Application.Interfaces;
using PS.FreeBookHub_Lite.OrderService.Common.Logging;

namespace PS.FreeBookHub_Lite.OrderService.Application.CQRS.Queries.GetAllOrdersByUserId
{
    public class GetAllOrdersByUserIdQueryHandler : IRequestHandler<GetAllOrdersByUserIdQuery, IEnumerable<OrderResponse>>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger<GetAllOrdersByUserIdQueryHandler> _logger;

        public GetAllOrdersByUserIdQueryHandler(
            IOrderRepository orderRepository,
            ILogger<GetAllOrdersByUserIdQueryHandler> logger)
        {
            _orderRepository = orderRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<OrderResponse>> Handle(GetAllOrdersByUserIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation(LoggerMessages.GetAllOrdersStarted, request.UserId);

            var orders = await _orderRepository.GetAllByUserIdAsync(request.UserId, cancellationToken);
            var response = orders.Adapt<IEnumerable<OrderResponse>>();

            _logger.LogInformation(LoggerMessages.GetAllOrdersSuccess, response.Count(), request.UserId);

            return response;
        }
    }
}

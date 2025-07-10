using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PS.FreeBookHub_Lite.OrderService.Application.DTOs;
using PS.FreeBookHub_Lite.OrderService.Application.Interfaces;
using PS.FreeBookHub_Lite.OrderService.Common.Configuration;
using PS.FreeBookHub_Lite.OrderService.Common.Events;
using PS.FreeBookHub_Lite.OrderService.Common.Events.Interfaces;
using PS.FreeBookHub_Lite.OrderService.Common.Logging;
using PS.FreeBookHub_Lite.OrderService.Domain.Entities;

namespace PS.FreeBookHub_Lite.OrderService.Application.CQRS.Commands.CreateOrder
{
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, OrderResponse>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger<CreateOrderCommandHandler> _logger;
        private readonly IEventPublisher _eventPublisher;
        private readonly RabbitMqConfig _config;

        public CreateOrderCommandHandler(
            IOrderRepository orderRepository,
            ILogger<CreateOrderCommandHandler> logger,
            IEventPublisher eventPublisher,
            IOptions<RabbitMqConfig> config)
        {
            _orderRepository = orderRepository;
            _logger = logger;
            _eventPublisher = eventPublisher;
            _config = config.Value;
        }

        public async Task<OrderResponse> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation(LoggerMessages.CreateOrderStarted, request.UserId);

            var order = new Order(request.UserId, request.ShippingAddress);

            foreach (var item in request.Items)
            {
                order.AddItem(item.BookId, item.UnitPrice, item.Quantity);
            }

            await _orderRepository.AddAsync(order, cancellationToken);

            var orderCreatedEvent = new OrderCreatedEvent(
                OrderId: order.Id,
                UserId: order.UserId,
                Amount: order.TotalPrice,
                CreatedAt: DateTime.UtcNow
            );

            await _eventPublisher.PublishAsync(orderCreatedEvent, routingKey: _config.RoutingKeys.OrderCreatedRoutingKey, cancellationToken);

            _logger.LogInformation(LoggerMessages.CreateOrderSuccess, order.Id, order.UserId);

            return order.Adapt<OrderResponse>();
        }
    }
}

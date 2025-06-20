using Mapster;
using Microsoft.Extensions.Logging;
using PS.FreeBookHub_Lite.OrderService.Application.Clients;
using PS.FreeBookHub_Lite.OrderService.Application.DTOs;
using PS.FreeBookHub_Lite.OrderService.Application.Interfaces;
using PS.FreeBookHub_Lite.OrderService.Application.Services.Interfaces;
using PS.FreeBookHub_Lite.OrderService.Common.Logging;
using PS.FreeBookHub_Lite.OrderService.Domain.Entities;
using PS.FreeBookHub_Lite.OrderService.Domain.Exceptions.Order;

namespace PS.FreeBookHub_Lite.OrderService.Application.Services
{
    public class OrderBookService : IOrderBookService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IPaymentServiceClient _paymentClient;
        private readonly ILogger<OrderBookService> _logger;

        public OrderBookService(
            IOrderRepository orderRepository,
            IPaymentServiceClient paymentClient,
            ILogger<OrderBookService> logger)
        {
            _orderRepository = orderRepository;
            _paymentClient = paymentClient;
            _logger = logger;
        }

        public async Task<OrderResponse> CreateOrderAsync(CreateOrderRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation(LoggerMessages.CreateOrderStarted, request.UserId);

            var order = new Order(request.UserId, request.ShippingAddress);

            foreach (var item in request.Items)
            {
                order.AddItem(item.BookId, item.UnitPrice, item.Quantity);
            }

            await _orderRepository.AddAsync(order, cancellationToken);

            var paymentRequest = new CreatePaymentRequest()
            {
                OrderId = order.Id,
                UserId = order.UserId,
                Amount = order.TotalPrice
            };

            await _paymentClient.CreatePaymentAsync(paymentRequest, cancellationToken);

            order.MarkAsPaid();
            await _orderRepository.UpdateAsync(order, cancellationToken);

            _logger.LogInformation(LoggerMessages.CreateOrderSuccess, order.Id, order.UserId);

            return order.Adapt<OrderResponse>();
        }

        public async Task<IEnumerable<OrderResponse>> GetAllOrdersByUserIdAsync(Guid userId, CancellationToken cancellationToken)
        {
            _logger.LogInformation(LoggerMessages.GetAllOrdersStarted, userId);

            var orders = await _orderRepository.GetAllByUserIdAsync(userId, cancellationToken);
            var response = orders.Adapt<IEnumerable<OrderResponse>>();

            _logger.LogInformation(LoggerMessages.GetAllOrdersSuccess, response.Count(), userId);

            return response;
        }

        public async Task<OrderResponse?> GetOrderByIdAsync(Guid orderId, CancellationToken cancellationToken)
        {
            _logger.LogInformation(LoggerMessages.GetOrderByIdStarted, orderId);

            var order = await _orderRepository.GetByIdAsync(orderId, cancellationToken, true);

            if (order == null)
            {
                throw new OrderNotFoundException(orderId);
            }

            _logger.LogInformation(LoggerMessages.GetOrderByIdSuccess, orderId);

            return order?.Adapt<OrderResponse>();
        }

        public async Task CancelOrderAsync(Guid orderId, CancellationToken cancellationToken)
        {
            _logger.LogInformation(LoggerMessages.CancelOrderStarted, orderId);

            var order = await _orderRepository.GetByIdAsync(orderId, cancellationToken);
            if (order == null)
            {
                throw new OrderNotFoundException(orderId);
            }

            order.Cancel();
            await _orderRepository.UpdateAsync(order, cancellationToken);

            _logger.LogInformation(LoggerMessages.CancelOrderSuccess, orderId);
        }
    }
}
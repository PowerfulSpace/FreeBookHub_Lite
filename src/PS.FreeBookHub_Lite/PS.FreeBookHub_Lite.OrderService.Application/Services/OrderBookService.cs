using Mapster;
using PS.FreeBookHub_Lite.OrderService.Application.Clients;
using PS.FreeBookHub_Lite.OrderService.Application.DTOs;
using PS.FreeBookHub_Lite.OrderService.Application.Interfaces;
using PS.FreeBookHub_Lite.OrderService.Application.Services.Interfaces;
using PS.FreeBookHub_Lite.OrderService.Domain.Entities;
using PS.FreeBookHub_Lite.OrderService.Domain.Exceptions;

namespace PS.FreeBookHub_Lite.OrderService.Application.Services
{
    public class OrderBookService : IOrderBookService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IPaymentServiceClient _paymentClient;

        public OrderBookService(IOrderRepository orderRepository, IPaymentServiceClient paymentClient)
        {
            _orderRepository = orderRepository;
            _paymentClient = paymentClient;
        }

        public async Task<OrderResponse> CreateOrderAsync(CreateOrderRequest request, CancellationToken cancellationToken)
        {
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

            var paymentResult = await _paymentClient.CreatePaymentAsync(paymentRequest, cancellationToken);

            if (!paymentResult)
                throw new PaymentFailedException(order.Id);

            order.MarkAsPaid();
            await _orderRepository.UpdateAsync(order, cancellationToken);

            return order.Adapt<OrderResponse>();
        }

        public async Task<IEnumerable<OrderResponse>> GetAllOrdersByUserIdAsync(Guid userId, CancellationToken cancellationToken)
        {
            var orders = await _orderRepository.GetAllByUserIdAsync(userId, cancellationToken);
            return orders.Adapt<IEnumerable<OrderResponse>>();
        }

        public async Task<OrderResponse?> GetOrderByIdAsync(Guid orderId, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetByIdAsync(orderId, cancellationToken, true);
            return order?.Adapt<OrderResponse>();
        }

        public async Task CancelOrderAsync(Guid orderId, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetByIdAsync(orderId, cancellationToken);
            if (order == null)
                throw new KeyNotFoundException("Order not found");

            order.Cancel();
            await _orderRepository.UpdateAsync(order, cancellationToken);
        }
    }
}
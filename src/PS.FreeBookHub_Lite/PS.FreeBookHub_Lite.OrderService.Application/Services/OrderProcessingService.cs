using Mapster;
using PS.FreeBookHub_Lite.OrderService.Application.Clients;
using PS.FreeBookHub_Lite.OrderService.Application.DTOs;
using PS.FreeBookHub_Lite.OrderService.Application.Interfaces;
using PS.FreeBookHub_Lite.OrderService.Application.Services.Interfaces;
using PS.FreeBookHub_Lite.OrderService.Domain.Entities;
using PS.FreeBookHub_Lite.OrderService.Domain.Exceptions;

namespace PS.FreeBookHub_Lite.OrderService.Application.Services
{
    public class OrderProcessingService : IOrderProcessingService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IPaymentServiceClient _paymentClient;

        public OrderProcessingService(IOrderRepository orderRepository, IPaymentServiceClient paymentClient)
        {
            _orderRepository = orderRepository;
            _paymentClient = paymentClient;
        }

        public async Task<OrderDto> CreateOrderAsync(CreateOrderRequest request)
        {
            var order = new Order(request.UserId, request.ShippingAddress);

            foreach (var item in request.Items)
            {
                order.AddItem(item.BookId, item.UnitPrice, item.Quantity);
            }

            await _orderRepository.AddAsync(order);

            var paymentRequest = new CreatePaymentRequest()
            {
                OrderId = order.Id,
                Amount = order.TotalPrice
            };

            var paymentResult = await _paymentClient.CreatePaymentAsync(paymentRequest);

            if (!paymentResult)
                throw new PaymentFailedException(order.Id);

            order.MarkAsPaid();
            await _orderRepository.UpdateAsync(order);

            return order.Adapt<OrderDto>();
        }

        public async Task<IEnumerable<OrderDto>> GetAllOrdersByUserIdAsync(Guid userId)
        {
            var orders = await _orderRepository.GetAllByUserIdAsync(userId);
            return orders.Adapt<IEnumerable<OrderDto>>();
        }

        public async Task<OrderDto?> GetOrderByIdAsync(Guid orderId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId, true);
            return order?.Adapt<OrderDto>();
        }

        public async Task CancelOrderAsync(Guid orderId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null)
                throw new KeyNotFoundException("Order not found");

            order.Cancel();
            await _orderRepository.UpdateAsync(order);
        }
    }
}
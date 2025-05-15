using Mapster;
using PS.FreeBookHub_Lite.OrderService.Application.DTOs;
using PS.FreeBookHub_Lite.OrderService.Application.Interfaces;
using PS.FreeBookHub_Lite.OrderService.Application.Services.Interfaces;
using PS.FreeBookHub_Lite.OrderService.Domain.Entities;

namespace PS.FreeBookHub_Lite.OrderService.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;

        public OrderService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<OrderDto> CreateOrderAsync(CreateOrderRequest request)
        {
            var order = new Order(request.UserId, request.ShippingAddress);

            foreach (var item in request.Items)
            {
                order.AddItem(item.BookId, item.UnitPrice, item.Quantity);
            }

            await _orderRepository.AddAsync(order);

            return order.Adapt<OrderDto>();
        }

        public async Task<IEnumerable<OrderDto>> GetAllOrdersByUserIdAsync(Guid userId)
        {
            var orders = await _orderRepository.GetAllByUserIdAsync(userId);
            return orders.Adapt<IEnumerable<OrderDto>>();
        }

        public async Task<OrderDto?> GetByIdAsync(Guid orderId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
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
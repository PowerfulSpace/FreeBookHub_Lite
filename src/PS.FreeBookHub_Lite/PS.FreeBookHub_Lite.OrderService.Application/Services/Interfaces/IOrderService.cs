using PS.FreeBookHub_Lite.OrderService.Application.DTOs;

namespace PS.FreeBookHub_Lite.OrderService.Application.Services.Interfaces
{
    public interface IOrderService
    {
        Task<OrderDto> CreateOrderAsync(CreateOrderRequest request);
        Task<IEnumerable<OrderDto>> GetAllOrdersByUserIdAsync(Guid userId);
        Task<OrderDto?> GetOrderByIdAsync(Guid orderId);
        Task CancelOrderAsync(Guid orderId);
    }
}

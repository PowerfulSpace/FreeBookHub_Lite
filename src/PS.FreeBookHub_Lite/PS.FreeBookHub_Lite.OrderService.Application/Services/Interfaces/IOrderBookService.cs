using PS.FreeBookHub_Lite.OrderService.Application.DTOs;

namespace PS.FreeBookHub_Lite.OrderService.Application.Services.Interfaces
{
    public interface IOrderBookService
    {
        Task<OrderResponse> CreateOrderAsync(CreateOrderRequest request);
        Task<IEnumerable<OrderResponse>> GetAllOrdersByUserIdAsync(Guid userId);
        Task<OrderResponse?> GetOrderByIdAsync(Guid orderId);
        Task CancelOrderAsync(Guid orderId);
    }
}

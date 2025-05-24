using PS.FreeBookHub_Lite.OrderService.Application.DTOs;

namespace PS.FreeBookHub_Lite.OrderService.Application.Services.Interfaces
{
    public interface IOrderBookService
    {
        Task<OrderResponse> CreateOrderAsync(CreateOrderRequest request, CancellationToken cancellationToken);
        Task<IEnumerable<OrderResponse>> GetAllOrdersByUserIdAsync(Guid userId, CancellationToken cancellationToken);
        Task<OrderResponse?> GetOrderByIdAsync(Guid orderId, CancellationToken cancellationToken);
        Task CancelOrderAsync(Guid orderId, CancellationToken cancellationToken);
    }
}

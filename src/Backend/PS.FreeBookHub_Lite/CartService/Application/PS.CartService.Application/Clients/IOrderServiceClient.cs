using PS.CartService.Application.DTOs.Order;

namespace PS.CartService.Application.Clients
{
    public interface IOrderServiceClient
    {
        Task<OrderResponse> CreateOrderAsync(CreateOrderRequest request, CancellationToken cancellationToken);
    }
}

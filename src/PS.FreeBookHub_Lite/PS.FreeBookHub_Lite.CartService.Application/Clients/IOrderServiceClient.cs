using PS.FreeBookHub_Lite.CartService.Application.DTOs.Order;

namespace PS.FreeBookHub_Lite.CartService.Application.Clients
{
    public interface IOrderServiceClient
    {
        Task<OrderResponse> CreateOrderAsync(CreateOrderRequest request);
    }
}

using PS.FreeBookHub_Lite.CartService.Application.DTOs;

namespace PS.FreeBookHub_Lite.CartService.Application.Clients
{
    public interface IOrderServiceClient
    {
        Task<OrderDto> CreateOrderAsync(CreateOrderRequest request);
    }
}

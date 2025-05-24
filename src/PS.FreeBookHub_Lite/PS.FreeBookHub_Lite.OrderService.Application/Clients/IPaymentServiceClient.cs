using PS.FreeBookHub_Lite.OrderService.Application.DTOs;

namespace PS.FreeBookHub_Lite.OrderService.Application.Clients
{
    public interface IPaymentServiceClient
    {
        Task<bool> CreatePaymentAsync(CreatePaymentRequest request, CancellationToken cancellationToken);
    }
}

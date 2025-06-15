using PS.FreeBookHub_Lite.OrderService.Application.DTOs;

namespace PS.FreeBookHub_Lite.OrderService.Application.Clients
{
    public interface IPaymentServiceClient
    {
        Task CreatePaymentAsync(CreatePaymentRequest request, CancellationToken cancellationToken);
    }
}

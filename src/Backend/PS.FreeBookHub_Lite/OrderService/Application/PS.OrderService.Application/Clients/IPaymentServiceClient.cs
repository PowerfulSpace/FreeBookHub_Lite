using PS.OrderService.Application.DTOs;

namespace PS.OrderService.Application.Clients
{
    public interface IPaymentServiceClient
    {
        Task CreatePaymentAsync(CreatePaymentRequest request, CancellationToken cancellationToken);
    }
}

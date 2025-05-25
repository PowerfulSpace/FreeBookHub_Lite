using PS.FreeBookHub_Lite.PaymentService.Application.DTOs;

namespace PS.FreeBookHub_Lite.PaymentService.Application.Services.Interfaces
{
    public interface IPaymentBookService
    {
        Task<PaymentResponse> ProcessPaymentAsync(CreatePaymentRequest request, CancellationToken cancellationToken);
        Task<PaymentResponse?> GetPaymentByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<IEnumerable<PaymentResponse>> GetPaymentsByOrderIdAsync(Guid orderId, CancellationToken cancellationToken);
    }
}

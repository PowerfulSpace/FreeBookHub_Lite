using PS.FreeBookHub_Lite.PaymentService.Application.DTOs;

namespace PS.FreeBookHub_Lite.PaymentService.Application.Services.Interfaces
{
    public interface IPaymentProcessingService
    {
        Task<PaymentResponse> ProcessPaymentAsync(CreatePaymentRequest request);
        Task<PaymentResponse?> GetPaymentByIdAsync(Guid id);
        Task<IEnumerable<PaymentResponse>> GetPaymentsByOrderIdAsync(Guid orderId);
    }
}

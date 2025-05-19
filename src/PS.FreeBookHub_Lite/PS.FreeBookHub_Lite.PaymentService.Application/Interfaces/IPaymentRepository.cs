using PS.FreeBookHub_Lite.PaymentService.Domain.Entities;

namespace PS.FreeBookHub_Lite.PaymentService.Application.Interfaces
{
    public interface IPaymentRepository
    {
        Task<Payment?> GetByIdAsync(Guid id, bool asNoTracking = false);
        Task<IEnumerable<Payment>> GetByOrderIdAsync(Guid orderId);
        Task AddAsync(Payment payment);
    }
}

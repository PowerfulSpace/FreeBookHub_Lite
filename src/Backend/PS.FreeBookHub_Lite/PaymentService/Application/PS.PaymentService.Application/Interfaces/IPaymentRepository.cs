using PS.PaymentService.Domain.Entities;

namespace PS.PaymentService.Application.Interfaces
{
    public interface IPaymentRepository
    {
        Task<Payment?> GetByIdAsync(Guid id, CancellationToken cancellationToken, bool asNoTracking = false);
        Task<IEnumerable<Payment>> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken);
        Task AddAsync(Payment payment, CancellationToken cancellationToken);
    }
}

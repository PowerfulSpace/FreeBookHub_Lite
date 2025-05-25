﻿using PS.FreeBookHub_Lite.PaymentService.Domain.Entities;

namespace PS.FreeBookHub_Lite.PaymentService.Application.Interfaces
{
    public interface IPaymentRepository
    {
        Task<Payment?> GetByIdAsync(Guid id, CancellationToken cancellationToken, bool asNoTracking = false);
        Task<IEnumerable<Payment>> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken);
        Task AddAsync(Payment payment, CancellationToken cancellationToken);
    }
}

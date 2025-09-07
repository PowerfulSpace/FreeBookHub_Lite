using Microsoft.EntityFrameworkCore;
using PS.PaymentService.Application.Interfaces;
using PS.PaymentService.Domain.Entities;

namespace PS.PaymentService.Infrastructure.Persistence.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly PaymentDbContext _context;

        public PaymentRepository(PaymentDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Payment payment, CancellationToken cancellationToken)
        {
            await _context.Payments.AddAsync(payment, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<Payment?> GetByIdAsync(Guid id, CancellationToken cancellationToken, bool asNoTracking = false)
        {
            IQueryable<Payment> payments = _context.Payments;

            if(asNoTracking)
            {
                payments = payments.AsNoTracking();
            }

            return await payments.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<Payment>> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken)
        {
            return await _context.Payments
                .AsNoTracking()
                .Where(p => p.OrderId == orderId)
                .ToListAsync(cancellationToken);
        }
    }
}

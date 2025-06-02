using Mapster;
using PS.FreeBookHub_Lite.PaymentService.Application.DTOs;
using PS.FreeBookHub_Lite.PaymentService.Application.Interfaces;
using PS.FreeBookHub_Lite.PaymentService.Application.Services.Interfaces;
using PS.FreeBookHub_Lite.PaymentService.Domain.Entities;

namespace PS.FreeBookHub_Lite.PaymentService.Application.Services
{
    public class PaymentBookService : IPaymentBookService
    {
        private readonly IPaymentRepository _paymentRepository;

        public PaymentBookService(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        public async Task<PaymentResponse> ProcessPaymentAsync(CreatePaymentRequest request, CancellationToken cancellationToken)
        {
            var payment = new Payment(request.OrderId, request.UserId, request.Amount);

            payment.MarkAsCompleted();

            await _paymentRepository.AddAsync(payment, cancellationToken);

            return payment.Adapt<PaymentResponse>();
        }

        public async Task<PaymentResponse?> GetPaymentByIdAsync(Guid id, Guid currentUserId, CancellationToken cancellationToken)
        {
            var payment = await _paymentRepository.GetByIdAsync(id, cancellationToken, true);

            if (payment is null || payment.UserId != currentUserId)
                return null;

            return payment.Adapt<PaymentResponse>();
        }

        public async Task<IEnumerable<PaymentResponse>> GetPaymentsByOrderIdAsync(Guid orderId, Guid currentUserId, CancellationToken cancellationToken)
        {
            var payments = await _paymentRepository.GetByOrderIdAsync(orderId, cancellationToken);

            if (payments.Any(p => p.UserId != currentUserId))
                return Enumerable.Empty<PaymentResponse>();

            return payments
                .Select(p => p.Adapt<PaymentResponse>())
                .ToList();
        }
    }
}

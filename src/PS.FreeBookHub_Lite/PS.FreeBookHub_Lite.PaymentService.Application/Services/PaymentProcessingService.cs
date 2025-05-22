using Mapster;
using PS.FreeBookHub_Lite.PaymentService.Application.DTOs;
using PS.FreeBookHub_Lite.PaymentService.Application.Interfaces;
using PS.FreeBookHub_Lite.PaymentService.Application.Services.Interfaces;
using PS.FreeBookHub_Lite.PaymentService.Domain.Entities;

namespace PS.FreeBookHub_Lite.PaymentService.Application.Services
{
    public class PaymentProcessingService : IPaymentProcessingService
    {
        private readonly IPaymentRepository _paymentRepository;

        public PaymentProcessingService(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        public async Task<PaymentResponse> ProcessPaymentAsync(CreatePaymentRequest request)
        {
            var payment = new Payment(request.OrderId, request.UserId, request.Amount);

            payment.MarkAsCompleted();

            await _paymentRepository.AddAsync(payment);

            return payment.Adapt<PaymentResponse>();
        }

        public async Task<PaymentResponse?> GetPaymentByIdAsync(Guid id)
        {
            var payment = await _paymentRepository.GetByIdAsync(id, true);
            if (payment is null) return null;

            return payment.Adapt<PaymentResponse>();
        }

        public async Task<IEnumerable<PaymentResponse>> GetPaymentsByOrderIdAsync(Guid orderId)
        {
            var payments = await _paymentRepository.GetByOrderIdAsync(orderId);
            return payments
                .Select(p => p.Adapt<PaymentResponse>())
                .ToList();
        }
    }
}

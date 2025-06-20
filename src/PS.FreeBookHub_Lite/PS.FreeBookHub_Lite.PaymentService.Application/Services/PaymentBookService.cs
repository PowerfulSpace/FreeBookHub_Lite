using Mapster;
using Microsoft.Extensions.Logging;
using PS.FreeBookHub_Lite.PaymentService.Application.DTOs;
using PS.FreeBookHub_Lite.PaymentService.Application.Interfaces;
using PS.FreeBookHub_Lite.PaymentService.Application.Services.Interfaces;
using PS.FreeBookHub_Lite.PaymentService.Common.Logging;
using PS.FreeBookHub_Lite.PaymentService.Domain.Entities;
using PS.FreeBookHub_Lite.PaymentService.Domain.Enums;
using PS.FreeBookHub_Lite.PaymentService.Domain.Exceptions.Payment;

namespace PS.FreeBookHub_Lite.PaymentService.Application.Services
{
    public class PaymentBookService : IPaymentBookService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly ILogger<PaymentBookService> _logger;

        public PaymentBookService(
            IPaymentRepository paymentRepository,
            ILogger<PaymentBookService> logger)
        {
            _paymentRepository = paymentRepository;
            _logger = logger;
        }

        public async Task<PaymentResponse> ProcessPaymentAsync(CreatePaymentRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation(LoggerMessages.ProcessPaymentStarted, request.OrderId, request.UserId);

            var existingPayment = await _paymentRepository.GetByOrderIdAsync(request.OrderId, cancellationToken);
            if (existingPayment.Any(p => p.Status == PaymentStatus.Completed))
            {
                throw new DuplicatePaymentException(request.OrderId);
            }

            var payment = new Payment(request.OrderId, request.UserId, request.Amount);

            payment.MarkAsCompleted();

            await _paymentRepository.AddAsync(payment, cancellationToken);

            _logger.LogInformation(LoggerMessages.ProcessPaymentSuccess, payment.Id, request.OrderId);

            return payment.Adapt<PaymentResponse>();
        }

        public async Task<PaymentResponse?> GetPaymentByIdAsync(Guid id, Guid currentUserId, CancellationToken cancellationToken)
        {
            _logger.LogInformation(LoggerMessages.GetPaymentByIdStarted, id);

            var payment = await _paymentRepository.GetByIdAsync(id, cancellationToken, true);

            if (payment is null)
            {
                throw new PaymentNotFoundException(id);
            }
            if (payment.UserId != currentUserId)
            {
                throw new UnauthorizedPaymentAccessException(id, currentUserId);
            }

            _logger.LogInformation(LoggerMessages.GetPaymentByIdSuccess, id);

            return payment.Adapt<PaymentResponse>();
        }

        public async Task<IEnumerable<PaymentResponse>> GetPaymentsByOrderIdAsync(Guid orderId, Guid currentUserId, CancellationToken cancellationToken)
        {
            _logger.LogInformation(LoggerMessages.GetPaymentsByOrderStarted, orderId);

            var payments = await _paymentRepository.GetByOrderIdAsync(orderId, cancellationToken);

            if (!payments.Any())
            {
                throw new PaymentNotFoundException(orderId);
            }

            if (payments.Any(p => p.UserId != currentUserId))
            {
                throw new UnauthorizedPaymentAccessException(orderId, currentUserId);
            }

            _logger.LogInformation(LoggerMessages.GetPaymentsByOrderSuccess, orderId, payments.Count());

            return payments
                .Select(p => p.Adapt<PaymentResponse>())
                .ToList();
        }
    }
}
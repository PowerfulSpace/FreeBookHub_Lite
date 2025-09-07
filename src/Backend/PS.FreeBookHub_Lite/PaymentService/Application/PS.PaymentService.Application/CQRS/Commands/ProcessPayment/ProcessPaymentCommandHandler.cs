using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;
using PS.PaymentService.Application.DTOs;
using PS.PaymentService.Application.Interfaces;
using PS.PaymentService.Common.Logging;
using PS.PaymentService.Domain.Entities;
using PS.PaymentService.Domain.Enums;
using PS.PaymentService.Domain.Exceptions.Payment;

namespace PS.PaymentService.Application.CQRS.Commands.ProcessPayment
{
    public class ProcessPaymentCommandHandler : IRequestHandler<ProcessPaymentCommand, PaymentResponse>
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly ILogger<ProcessPaymentCommandHandler> _logger;

        public ProcessPaymentCommandHandler(
            IPaymentRepository paymentRepository,
            ILogger<ProcessPaymentCommandHandler> logger)
        {
            _paymentRepository = paymentRepository;
            _logger = logger;
        }

        public async Task<PaymentResponse> Handle(ProcessPaymentCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation(LoggerMessages.ProcessPaymentStarted, request.OrderId, request.UserId);

            var existingPayments = await _paymentRepository.GetByOrderIdAsync(request.OrderId, cancellationToken);
            if (existingPayments.Any(p => p.Status == PaymentStatus.Completed))
            {
                throw new DuplicatePaymentException(request.OrderId);
            }

            var payment = new Payment(
                request.OrderId,
                request.UserId,
                request.Amount
            );

            payment.MarkAsCompleted();

            await _paymentRepository.AddAsync(payment, cancellationToken);

            _logger.LogInformation(LoggerMessages.ProcessPaymentSuccess, payment.Id, request.OrderId);

            return payment.Adapt<PaymentResponse>();
        }
    }
}

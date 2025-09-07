using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;
using PS.PaymentService.Application.DTOs;
using PS.PaymentService.Application.Interfaces;
using PS.PaymentService.Common.Logging;
using PS.PaymentService.Domain.Exceptions.Payment;

namespace PS.PaymentService.Application.CQRS.Queries.GetPaymentsByOrderId
{
    public class GetPaymentsByOrderIdQueryHandler : IRequestHandler<GetPaymentsByOrderIdQuery, IEnumerable<PaymentResponse>>
    {

        private readonly IPaymentRepository _paymentRepository;
        private readonly ILogger<GetPaymentsByOrderIdQueryHandler> _logger;

        public GetPaymentsByOrderIdQueryHandler(
            IPaymentRepository paymentRepository,
            ILogger<GetPaymentsByOrderIdQueryHandler> logger)
        {
            _paymentRepository = paymentRepository;
            _logger = logger;
        }
        public async Task<IEnumerable<PaymentResponse>> Handle(GetPaymentsByOrderIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation(LoggerMessages.GetPaymentsByOrderStarted, request.OrderId);

            var payments = await _paymentRepository.GetByOrderIdAsync(request.OrderId, cancellationToken);

            if (!payments.Any())
            {
                throw new PaymentNotFoundException(request.OrderId);
            }

            if (payments.Any(p => p.UserId != request.CurrentUserId))
            {
                throw new UnauthorizedPaymentAccessException(request.OrderId, request.CurrentUserId);
            }

            _logger.LogInformation(LoggerMessages.GetPaymentsByOrderSuccess, request.OrderId, payments.Count());

            return payments
                .Select(p => p.Adapt<PaymentResponse>())
                .ToList();
        }
    }
}

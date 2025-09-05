using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;
using PS.FreeBookHub_Lite.PaymentService.Application.DTOs;
using PS.FreeBookHub_Lite.PaymentService.Application.Interfaces;
using PS.FreeBookHub_Lite.PaymentService.Common.Logging;
using PS.FreeBookHub_Lite.PaymentService.Domain.Exceptions.Payment;

namespace PS.FreeBookHub_Lite.PaymentService.Application.CQRS.Queries.GetPaymentById
{
    public class GetPaymentByIdQueryHandler : IRequestHandler<GetPaymentByIdQuery, PaymentResponse>
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly ILogger<GetPaymentByIdQueryHandler> _logger;

        public GetPaymentByIdQueryHandler(
            IPaymentRepository paymentRepository,
            ILogger<GetPaymentByIdQueryHandler> logger)
        {
            _paymentRepository = paymentRepository;
            _logger = logger;
        }

        public async Task<PaymentResponse> Handle(GetPaymentByIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation(LoggerMessages.GetPaymentByIdStarted, request.PaymentId);

            var payment = await _paymentRepository.GetByIdAsync(request.PaymentId, cancellationToken, true);

            if (payment is null)
            {
                throw new PaymentNotFoundException(request.PaymentId);
            }

            if (payment.UserId != request.CurrentUserId)
            {
                throw new UnauthorizedPaymentAccessException(request.PaymentId, request.CurrentUserId);
            }

            _logger.LogInformation(LoggerMessages.GetPaymentByIdSuccess, request.PaymentId);

            return payment.Adapt<PaymentResponse>();
        }
    }
}

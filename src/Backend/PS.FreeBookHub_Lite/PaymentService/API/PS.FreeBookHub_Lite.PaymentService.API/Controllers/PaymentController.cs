using Mapster;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PS.FreeBookHub_Lite.PaymentService.Application.CQRS.Commands.ProcessPayment;
using PS.FreeBookHub_Lite.PaymentService.Application.CQRS.Queries.GetPaymentById;
using PS.FreeBookHub_Lite.PaymentService.Application.CQRS.Queries.GetPaymentsByOrderId;
using PS.FreeBookHub_Lite.PaymentService.Application.DTOs;
using PS.FreeBookHub_Lite.PaymentService.Domain.Exceptions.User;
using Swashbuckle.AspNetCore.Annotations;

namespace PS.FreeBookHub_Lite.PaymentService.API.Controllers
{
    [Authorize(Policy = "User")]
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PaymentController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Authorize(Policy = "InternalOnly")]
        [HttpPost]
        [SwaggerOperation(Summary = "Создание платежа", Description = "Обрабатывает и создает новый платеж")]
        public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentRequest request, CancellationToken ct)
        {
            var userId = GetUserIdFromClaimsOrThrow();
            //request.UserId = userId;

            //var result = await _paymentService.ProcessPaymentAsync(request, ct);

            var command = request.Adapt<ProcessPaymentCommand>();
            command.UserId = userId;

            var payment = await _mediator.Send(command, ct);

            return CreatedAtAction(nameof(GetPaymentById), new { id = payment.Id }, payment);
        }

        [HttpGet("{id:guid}")]
        [SwaggerOperation(Summary = "Получение платежа по ID", Description = "Возвращает информацию о платеже по его идентификатору")]
        public async Task<IActionResult> GetPaymentById(Guid id, CancellationToken ct)
        {
            var userId = GetUserIdFromClaimsOrThrow();

            //var result = await _paymentService.GetPaymentByIdAsync(id, userId, ct);

            var query = new GetPaymentByIdQuery(id, userId);

            var result = await _mediator.Send(query, ct);

            if (result is null)
                return NotFound();

            return Ok(result);
        }

        [HttpGet("by-order/{orderId:guid}")]
        [SwaggerOperation(Summary = "Получение платежей по заказу", Description = "Возвращает все платежи, связанные с указанным заказом")]
        public async Task<IActionResult> GetPaymentsByOrderId(Guid orderId, CancellationToken ct)
        {
            var userId = GetUserIdFromClaimsOrThrow();

            //var results = await _paymentService.GetPaymentsByOrderIdAsync(orderId, userId, ct);

            var query = new GetPaymentsByOrderIdQuery(orderId, userId);

            var results = await _mediator.Send(query, ct);

            return Ok(results);
        }

        private Guid GetUserIdFromClaimsOrThrow()
        {
            var userId = User.FindFirst("sub")?.Value
              ?? User.FindFirst("nameidentifier")?.Value;

            if (!Guid.TryParse(userId, out var result))
                throw new InvalidUserIdentifierException(userId ?? "null");

            return result;
        }
    }
}
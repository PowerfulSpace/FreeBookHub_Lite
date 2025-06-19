using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PS.FreeBookHub_Lite.PaymentService.Application.DTOs;
using PS.FreeBookHub_Lite.PaymentService.Application.Services.Interfaces;
using PS.FreeBookHub_Lite.PaymentService.Domain.Exceptions.User;
using Swashbuckle.AspNetCore.Annotations;

namespace PS.FreeBookHub_Lite.PaymentService.API.Controllers
{
    [Authorize(Policy = "User")]
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentBookService _paymentService;

        public PaymentController(IPaymentBookService paymentService)
        {
            _paymentService = paymentService;
        }

        [Authorize(Policy = "InternalOnly")]
        [HttpPost]
        [SwaggerOperation(Summary = "Создание платежа", Description = "Обрабатывает и создает новый платеж")]
        public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentRequest request, CancellationToken cancellationToken)
        {
            var userId = GetUserIdFromClaimsOrThrow();
            request.UserId = userId;

            var result = await _paymentService.ProcessPaymentAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetPaymentById), new { id = result.Id }, result);
        }

        [HttpGet("{id:guid}")]
        [SwaggerOperation(Summary = "Получение платежа по ID", Description = "Возвращает информацию о платеже по его идентификатору")]
        public async Task<IActionResult> GetPaymentById(Guid id, CancellationToken cancellationToken)
        {
            var userId = GetUserIdFromClaimsOrThrow();

            var result = await _paymentService.GetPaymentByIdAsync(id, userId, cancellationToken);
            if (result is null)
                return NotFound();

            return Ok(result);
        }

        [HttpGet("by-order/{orderId:guid}")]
        [SwaggerOperation(Summary = "Получение платежей по заказу", Description = "Возвращает все платежи, связанные с указанным заказом")]
        public async Task<IActionResult> GetPaymentsByOrderId(Guid orderId, CancellationToken cancellationToken)
        {
            var userId = GetUserIdFromClaimsOrThrow();

            var results = await _paymentService.GetPaymentsByOrderIdAsync(orderId, userId, cancellationToken);
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
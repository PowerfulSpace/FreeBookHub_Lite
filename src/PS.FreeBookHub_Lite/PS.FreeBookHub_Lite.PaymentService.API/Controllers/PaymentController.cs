using Microsoft.AspNetCore.Mvc;
using PS.FreeBookHub_Lite.PaymentService.Application.DTOs;
using PS.FreeBookHub_Lite.PaymentService.Application.Services.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace PS.FreeBookHub_Lite.PaymentService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentBookService _paymentService;

        public PaymentController(IPaymentBookService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Создание платежа", Description = "Обрабатывает и создает новый платеж")]
        public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentRequest request)
        {
            var result = await _paymentService.ProcessPaymentAsync(request);
            return CreatedAtAction(nameof(GetPaymentById), new { id = result.Id }, result);
        }

        [HttpGet("{id:guid}")]
        [SwaggerOperation(Summary = "Получение платежа по ID", Description = "Возвращает информацию о платеже по его идентификатору")]
        public async Task<IActionResult> GetPaymentById(Guid id)
        {
            var result = await _paymentService.GetPaymentByIdAsync(id);
            if (result is null)
                return NotFound();

            return Ok(result);
        }

        [HttpGet("by-order/{orderId:guid}")]
        [SwaggerOperation(Summary = "Получение платежей по заказу", Description = "Возвращает все платежи, связанные с указанным заказом")]
        public async Task<IActionResult> GetPaymentsByOrderId(Guid orderId)
        {
            var results = await _paymentService.GetPaymentsByOrderIdAsync(orderId);
            return Ok(results);
        }
    }
}

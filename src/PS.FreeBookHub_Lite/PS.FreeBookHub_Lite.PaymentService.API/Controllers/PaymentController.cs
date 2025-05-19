using Microsoft.AspNetCore.Mvc;
using PS.FreeBookHub_Lite.PaymentService.Application.DTOs;
using PS.FreeBookHub_Lite.PaymentService.Application.Services.Interfaces;

namespace PS.FreeBookHub_Lite.PaymentService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentProcessingService _paymentService;

        public PaymentController(IPaymentProcessingService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost]
        public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentRequest request)
        {
            var result = await _paymentService.ProcessPaymentAsync(request);
            return CreatedAtAction(nameof(GetPaymentById), new { id = result.Id }, result);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetPaymentById(Guid id)
        {
            var result = await _paymentService.GetPaymentByIdAsync(id);
            if (result is null)
                return NotFound();

            return Ok(result);
        }

        [HttpGet("by-order/{orderId:guid}")]
        public async Task<IActionResult> GetPaymentsByOrderId(Guid orderId)
        {
            var results = await _paymentService.GetPaymentsByOrderIdAsync(orderId);
            return Ok(results);
        }
    }
}

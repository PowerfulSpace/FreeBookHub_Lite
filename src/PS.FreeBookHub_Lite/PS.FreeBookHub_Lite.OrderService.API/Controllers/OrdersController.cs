using Microsoft.AspNetCore.Mvc;
using PS.FreeBookHub_Lite.OrderService.Application.DTOs;
using PS.FreeBookHub_Lite.OrderService.Application.Services.Interfaces;

namespace PS.FreeBookHub_Lite.OrderService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderProcessingService _orderService;

        public OrdersController(IOrderProcessingService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateOrderRequest request)
        {
            var order = await _orderService.CreateOrderAsync(request);
            return CreatedAtAction(nameof(GetById), new { orderId = order.Id }, order);
        }

        [HttpGet("{orderId:guid}")]
        public async Task<IActionResult> GetById(Guid orderId)
        {
            var order = await _orderService.GetOrderByIdAsync(orderId);
            if (order == null)
                return NotFound();

            return Ok(order);
        }

        [HttpGet("user/{userId:guid}")]
        public async Task<IActionResult> GetAllByUserId(Guid userId)
        {
            var orders = await _orderService.GetAllOrdersByUserIdAsync(userId);
            return Ok(orders);
        }

        [HttpPost("{orderId:guid}/cancel")]
        public async Task<IActionResult> Cancel(Guid orderId)
        {
            try
            {
                await _orderService.CancelOrderAsync(orderId);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

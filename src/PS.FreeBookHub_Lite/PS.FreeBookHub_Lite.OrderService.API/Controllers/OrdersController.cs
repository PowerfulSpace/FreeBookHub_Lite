using Microsoft.AspNetCore.Mvc;
using PS.FreeBookHub_Lite.OrderService.Application.DTOs;
using PS.FreeBookHub_Lite.OrderService.Application.Services.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

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
        [SwaggerOperation(Summary = "Создание нового заказа", Description = "Создает новый заказ на основе переданных данных")]
        public async Task<IActionResult> Create([FromBody] CreateOrderRequest request)
        {
            var order = await _orderService.CreateOrderAsync(request);
            return CreatedAtAction(nameof(GetById), new { orderId = order.Id }, order);
        }

        [HttpGet("{orderId:guid}")]
        [SwaggerOperation(Summary = "Получение заказа по ID", Description = "Возвращает детали заказа по его идентификатору")]
        public async Task<IActionResult> GetById(Guid orderId)
        {
            var order = await _orderService.GetOrderByIdAsync(orderId);
            if (order == null)
                return NotFound();

            return Ok(order);
        }

        [HttpGet("user/{userId:guid}")]
        [SwaggerOperation(Summary = "Получение заказов пользователя", Description = "Возвращает список всех заказов, принадлежащих указанному пользователю")]
        public async Task<IActionResult> GetAllByUserId(Guid userId)
        {
            var orders = await _orderService.GetAllOrdersByUserIdAsync(userId);
            return Ok(orders);
        }

        [HttpDelete("{orderId:guid}/cancel")]
        [SwaggerOperation(Summary = "Отмена заказа", Description = "Отменяет заказ, если это возможно (например, если он еще не обработан)")]
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

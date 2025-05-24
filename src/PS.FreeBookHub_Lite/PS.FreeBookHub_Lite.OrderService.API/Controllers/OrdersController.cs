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
        private readonly IOrderBookService _orderService;

        public OrdersController(IOrderBookService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Создание нового заказа", Description = "Создает новый заказ на основе переданных данных")]
        public async Task<IActionResult> Create([FromBody] CreateOrderRequest request, CancellationToken cancellationToken)
        {
            var order = await _orderService.CreateOrderAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { orderId = order.Id }, order);
        }

        [HttpGet("{orderId:guid}")]
        [SwaggerOperation(Summary = "Получение заказа по ID", Description = "Возвращает детали заказа по его идентификатору")]
        public async Task<IActionResult> GetById(Guid orderId, CancellationToken cancellationToken)
        {
            var order = await _orderService.GetOrderByIdAsync(orderId, cancellationToken);
            if (order == null)
                return NotFound();

            return Ok(order);
        }

        [HttpGet("user/{userId:guid}")]
        [SwaggerOperation(Summary = "Получение заказов пользователя", Description = "Возвращает список всех заказов, принадлежащих указанному пользователю")]
        public async Task<IActionResult> GetAllByUserId(Guid userId, CancellationToken cancellationToken)
        {
            var orders = await _orderService.GetAllOrdersByUserIdAsync(userId, cancellationToken);
            return Ok(orders);
        }

        [HttpDelete("{orderId:guid}/cancel")]
        [SwaggerOperation(Summary = "Отмена заказа", Description = "Отменяет заказ, если это возможно (например, если он еще не обработан)")]
        public async Task<IActionResult> Cancel(Guid orderId, CancellationToken cancellationToken)
        {
            try
            {
                await _orderService.CancelOrderAsync(orderId, cancellationToken);
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

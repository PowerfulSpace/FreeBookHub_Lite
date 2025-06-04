using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PS.FreeBookHub_Lite.OrderService.Application.DTOs;
using PS.FreeBookHub_Lite.OrderService.Application.Services.Interfaces;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

namespace PS.FreeBookHub_Lite.OrderService.API.Controllers
{
    [Authorize(Policy = "User")]
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
        [SwaggerOperation(Summary = "Создание нового заказа", Description = "Создает новый заказ от текущего пользователя")]
        public async Task<IActionResult> Create([FromBody] CreateOrderRequest request, CancellationToken cancellationToken)
        {
            var userId = GetUserIdFromClaimsOrThrow();
            request.UserId = userId;

            var order = await _orderService.CreateOrderAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { orderId = order.Id }, order);
        }

        [HttpGet("{orderId:guid}")]
        [SwaggerOperation(Summary = "Получение заказа по ID", Description = "Возвращает детали заказа, если он принадлежит текущему пользователю")]
        public async Task<IActionResult> GetById(Guid orderId, CancellationToken cancellationToken)
        {
            var userId = GetUserIdFromClaimsOrThrow();
            var order = await _orderService.GetOrderByIdAsync(orderId, cancellationToken);

            if (order == null)
                return NotFound();

            if (order.UserId != userId)
                return Forbid();

            return Ok(order);
        }

        [HttpGet("my")]
        [SwaggerOperation(Summary = "Получение всех заказов текущего пользователя", Description = "Возвращает список всех заказов текущего пользователя")]
        public async Task<IActionResult> GetAllMyOrders(CancellationToken cancellationToken)
        {
            var userId = GetUserIdFromClaimsOrThrow();
            var orders = await _orderService.GetAllOrdersByUserIdAsync(userId, cancellationToken);
            return Ok(orders);
        }

        [HttpDelete("{orderId:guid}/cancel")]
        [SwaggerOperation(Summary = "Отмена заказа", Description = "Отменяет заказ, если он принадлежит текущему пользователю и может быть отменён")]
        public async Task<IActionResult> Cancel(Guid orderId, CancellationToken cancellationToken)
        {
            var userId = GetUserIdFromClaimsOrThrow();
            var order = await _orderService.GetOrderByIdAsync(orderId, cancellationToken);

            if (order == null)
                return NotFound();

            if (order.UserId != userId)
                return Forbid();

            await _orderService.CancelOrderAsync(orderId, cancellationToken);
            return NoContent();
        }

        private Guid GetUserIdFromClaimsOrThrow()
        {
            var userId = User.FindFirst("sub")?.Value
              ?? User.FindFirst("nameidentifier")?.Value;

            //var userId = User.FindFirst("sub")?.Value
            //          ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(userId, out var result))
                throw new UnauthorizedAccessException("Invalid user ID format");

            return result;
        }
    }
}

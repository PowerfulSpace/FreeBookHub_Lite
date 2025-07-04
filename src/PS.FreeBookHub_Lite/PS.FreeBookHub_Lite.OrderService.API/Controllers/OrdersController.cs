using Mapster;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PS.FreeBookHub_Lite.OrderService.Application.CQRS.Commands.CancelOrder;
using PS.FreeBookHub_Lite.OrderService.Application.CQRS.Commands.CreateOrder;
using PS.FreeBookHub_Lite.OrderService.Application.CQRS.Queries.GetAllOrdersByUserId;
using PS.FreeBookHub_Lite.OrderService.Application.CQRS.Queries.GetOrderById;
using PS.FreeBookHub_Lite.OrderService.Application.DTOs;
using PS.FreeBookHub_Lite.OrderService.Domain.Exceptions.User;
using Swashbuckle.AspNetCore.Annotations;

namespace PS.FreeBookHub_Lite.OrderService.API.Controllers
{
    [Authorize(Policy = "User")]
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OrdersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Authorize(Policy = "InternalOnly")]
        [HttpPost]
        [SwaggerOperation(Summary = "Создание нового заказа", Description = "Создает новый заказ от текущего пользователя")]
        public async Task<IActionResult> Create([FromBody] CreateOrderRequest request, CancellationToken ct)
        {
            var userId = GetUserIdFromClaimsOrThrow();

            var command = request.Adapt<CreateOrderCommand>();
            command.UserId = userId;

            var order = await _mediator.Send(command, ct);

            return CreatedAtAction(nameof(GetById), new { orderId = order.Id }, order);
        }

        [HttpGet("{orderId:guid}")]
        [SwaggerOperation(Summary = "Получение заказа по ID", Description = "Возвращает детали заказа, если он принадлежит текущему пользователю")]
        public async Task<IActionResult> GetById(Guid orderId, CancellationToken ct)
        {
            var userId = GetUserIdFromClaimsOrThrow();

            var query = new GetOrderByIdQuery(orderId);

            var order = await _mediator.Send(query, ct);

            if (order == null)
                return NotFound();

            if (order.UserId != userId)
                return Forbid();

            return Ok(order);
        }

        [HttpGet("my")]
        [SwaggerOperation(Summary = "Получение всех заказов текущего пользователя", Description = "Возвращает список всех заказов текущего пользователя")]
        public async Task<IActionResult> GetAllMyOrders(CancellationToken ct)
        {
            var userId = GetUserIdFromClaimsOrThrow();

            //var orders = await _orderService.GetAllOrdersByUserIdAsync(userId, ct);

            var query = new GetAllOrdersByUserIdQuery(userId);

            var orders = await _mediator.Send(query, ct);

            return Ok(orders);
        }

        [HttpDelete("{orderId:guid}/cancel")]
        [SwaggerOperation(Summary = "Отмена заказа", Description = "Отменяет заказ, если он принадлежит текущему пользователю и может быть отменён")]
        public async Task<IActionResult> Cancel(Guid orderId, CancellationToken ct)
        {
            var userId = GetUserIdFromClaimsOrThrow();

            //var order = await _orderService.GetOrderByIdAsync(orderId, ct);

            var query = new GetOrderByIdQuery(orderId);
            var order = await _mediator.Send(query, ct);

            if (order == null)
                return NotFound();

            if (order.UserId != userId)
                return Forbid();

            //await _orderService.CancelOrderAsync(orderId, ct);

            var command = new CancelOrderCommand(orderId);
            await _mediator.Send(command, ct);

            return NoContent();
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

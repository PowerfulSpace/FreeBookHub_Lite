using Mapster;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PS.CartService.Application.CQRS.Commands.AddItem;
using PS.CartService.Application.CQRS.Commands.Checkout;
using PS.CartService.Application.CQRS.Commands.ClearCart;
using PS.CartService.Application.CQRS.Commands.RemoveItem;
using PS.CartService.Application.CQRS.Commands.UpdateItemQuantity;
using PS.CartService.Application.CQRS.Queries.GetCart;
using PS.CartService.Application.DTOs.Cart;
using PS.CartService.Application.DTOs.Order;
using PS.CartService.Domain.Exceptions.User;
using Swashbuckle.AspNetCore.Annotations;

namespace PS.CartService.API.Controllers
{
    [Authorize(Policy = "User")]
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CartController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [HttpGet("my")]
        [SwaggerOperation(Summary = "Получение своей корзины", Description = "Возвращает содержимое корзины текущего пользователя")]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            var userId = GetUserIdFromClaimsOrThrow();

            var query = new GetCartQuery(userId);
            var cart = await _mediator.Send(query, cancellationToken);

            return Ok(cart);
        }

        [HttpPost("my/books")]
        [SwaggerOperation(Summary = "Добавление книги в корзину", Description = "Добавляет книгу в корзину текущего пользователя")]
        public async Task<IActionResult> AddBook([FromBody] AddItemRequest request, CancellationToken cancellationToken)
        {
            var userId = GetUserIdFromClaimsOrThrow();

            var command = request.BuildAdapter()
                        .AddParameters("UserId", userId)
                        .AdaptToType<AddItemCommand>();
            await _mediator.Send(command, cancellationToken);

            return NoContent();
        }

        [HttpPatch("my/items")]
        [SwaggerOperation(Summary = "Обновление количества книги", Description = "Изменяет количество книги в корзине текущего пользователя")]
        public async Task<IActionResult> UpdateBookQuantity([FromBody] UpdateItemQuantityRequest request, CancellationToken cancellationToken)
        {
            var userId = GetUserIdFromClaimsOrThrow();

            var command = request.BuildAdapter()
                       .AddParameters("UserId", userId)
                       .AdaptToType<UpdateItemQuantityCommand>();

            await _mediator.Send(command, cancellationToken);

            return NoContent();
        }

        [HttpDelete("my/items/{bookId:guid}")]
        [SwaggerOperation(Summary = "Удаление книги из корзины", Description = "Удаляет книгу из корзины текущего пользователя")]
        public async Task<IActionResult> RemoveBook(Guid bookId, CancellationToken cancellationToken)
        {
            var userId = GetUserIdFromClaimsOrThrow();

            var command = new RemoveItemCommand(userId, bookId);
            await _mediator.Send(command, cancellationToken);

            return NoContent();
        }

        [HttpPut("my/clear")]
        [SwaggerOperation(Summary = "Очистка корзины", Description = "Удаляет все книги из корзины текущего пользователя")]
        public async Task<IActionResult> ClearCart(CancellationToken cancellationToken)
        {
            var userId = GetUserIdFromClaimsOrThrow();

            var command = new ClearCartCommand(userId);
            await _mediator.Send(command, cancellationToken);

            return NoContent();
        }

        [HttpPost("checkout")]
        [SwaggerOperation(Summary = "Оформление заказа", Description = "Оформляет заказ на основе корзины текущего пользователя")]
        public async Task<IActionResult> Checkout([FromBody] CheckoutRequest request, CancellationToken cancellationToken)
        {
            var userId = GetUserIdFromClaimsOrThrow();

            var command = request.BuildAdapter()
                      .AddParameters("UserId", userId)
                      .AdaptToType<CheckoutCommand>();

            var order = await _mediator.Send(command, cancellationToken);

            return Ok(order);
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
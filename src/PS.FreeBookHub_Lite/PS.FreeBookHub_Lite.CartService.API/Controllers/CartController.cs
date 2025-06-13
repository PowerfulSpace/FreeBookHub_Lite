using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PS.FreeBookHub_Lite.CartService.Application.DTOs.Cart;
using PS.FreeBookHub_Lite.CartService.Application.DTOs.Order;
using PS.FreeBookHub_Lite.CartService.Application.Services.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace PS.FreeBookHub_Lite.CartService.API.Controllers
{
    [Authorize(Policy = "User")]
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly ICartBookService _cartService;

        public CartController(ICartBookService cartService)
        {
            _cartService = cartService;
        }

        
        [HttpGet("my")]
        [SwaggerOperation(Summary = "Получение своей корзины", Description = "Возвращает содержимое корзины текущего пользователя")]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            var userId = GetUserIdFromClaimsOrThrow();
            var cart = await _cartService.GetCartAsync(userId, cancellationToken);
            return Ok(cart);
        }

        [HttpPost("my/books")]
        [SwaggerOperation(Summary = "Добавление книги в корзину", Description = "Добавляет книгу в корзину текущего пользователя")]
        public async Task<IActionResult> AddBook([FromBody] AddItemRequest request, CancellationToken cancellationToken)
        {
            var userId = GetUserIdFromClaimsOrThrow();
            await _cartService.AddItemAsync(userId, request, cancellationToken);
            return NoContent();
        }

        [HttpPatch("my/items")]
        [SwaggerOperation(Summary = "Обновление количества книги", Description = "Изменяет количество книги в корзине текущего пользователя")]
        public async Task<IActionResult> UpdateBookQuantity([FromBody] UpdateItemQuantityRequest request, CancellationToken cancellationToken)
        {
            var userId = GetUserIdFromClaimsOrThrow();
            await _cartService.UpdateItemQuantityAsync(userId, request, cancellationToken);
            return NoContent();
        }

        [HttpDelete("my/items/{bookId:guid}")]
        [SwaggerOperation(Summary = "Удаление книги из корзины", Description = "Удаляет книгу из корзины текущего пользователя")]
        public async Task<IActionResult> RemoveBook(Guid bookId, CancellationToken cancellationToken)
        {
            var userId = GetUserIdFromClaimsOrThrow();
            await _cartService.RemoveItemAsync(userId, bookId, cancellationToken);
            return NoContent();
        }

        [HttpPut("my/clear")]
        [SwaggerOperation(Summary = "Очистка корзины", Description = "Удаляет все книги из корзины текущего пользователя")]
        public async Task<IActionResult> ClearCart(CancellationToken cancellationToken)
        {
            var userId = GetUserIdFromClaimsOrThrow();
            await _cartService.ClearCartAsync(userId, cancellationToken);
            return NoContent();
        }

        [HttpPost("checkout")]
        [SwaggerOperation(Summary = "Оформление заказа", Description = "Оформляет заказ на основе корзины текущего пользователя")]
        public async Task<IActionResult> Checkout([FromBody] CheckoutRequest request, CancellationToken cancellationToken)
        {
            var userId = GetUserIdFromClaimsOrThrow();
            var order = await _cartService.CheckoutAsync(userId, request.ShippingAddress, cancellationToken);
            return Ok(order);
        }

        private Guid GetUserIdFromClaimsOrThrow()
        {
            var userId = User.FindFirst("sub")?.Value
              ?? User.FindFirst("nameidentifier")?.Value;

            if (!Guid.TryParse(userId, out var result))
                throw new UnauthorizedAccessException("Invalid user ID format");

            return result;
        }
    }
}
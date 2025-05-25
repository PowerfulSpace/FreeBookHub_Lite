using Microsoft.AspNetCore.Mvc;
using PS.FreeBookHub_Lite.CartService.Application.DTOs.Cart;
using PS.FreeBookHub_Lite.CartService.Application.DTOs.Order;
using PS.FreeBookHub_Lite.CartService.Application.Services.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace PS.FreeBookHub_Lite.CartService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly ICartBookService _cartService;

        public CartController(ICartBookService cartService)
        {
            _cartService = cartService;
        }

        [HttpGet("{userId:guid}")]
        [SwaggerOperation(Summary = "Получение корзины пользователя", Description = "Возвращает содержимое корзины по UserId")]
        public async Task<IActionResult> Get(Guid userId, CancellationToken cancellationToken)
        {
            var cart = await _cartService.GetCartAsync(userId, cancellationToken);
            return Ok(cart);
        }

        [HttpPost("{userId:guid}/books")]
        [SwaggerOperation(Summary = "Добавление товара в корзину", Description = "Добавляет книгу в корзину пользователя")]
        public async Task<IActionResult> AddBook(Guid userId, [FromBody] AddItemRequest request, CancellationToken cancellationToken)
        {
            await _cartService.AddItemAsync(userId, request, cancellationToken);
            return NoContent();
        }

        [HttpPatch("{userId:guid}/items")]
        [SwaggerOperation(Summary = "Обновление количества товара", Description = "Изменяет количество книги в корзине пользователя")]
        public async Task<IActionResult> UpdateBookQuantity(Guid userId, [FromBody] UpdateItemQuantityRequest request, CancellationToken cancellationToken)
        {
            await _cartService.UpdateItemQuantityAsync(userId, request, cancellationToken);
            return NoContent();
        }

        [HttpDelete("{userId:guid}/items/{bookId:guid}")]
        [SwaggerOperation(Summary = "Удаление товара из корзины", Description = "Удаляет указанный товар из корзины пользователя")]
        public async Task<IActionResult> RemoveBook(Guid userId, Guid bookId, CancellationToken cancellationToken)
        {
            await _cartService.RemoveItemAsync(userId, bookId, cancellationToken);
            return NoContent();
        }

        [HttpPut("{userId:guid}")]
        [SwaggerOperation(Summary = "Очистка корзины", Description = "Удаляет все товары из корзины пользователя")]
        public async Task<IActionResult> ClearCart(Guid userId, CancellationToken cancellationToken)
        {
            await _cartService.ClearCartAsync(userId, cancellationToken);
            return NoContent();
        }

        [HttpPost("checkout")]
        [SwaggerOperation(Summary = "Оформление заказа из корзины", Description = "Выполняет оформление заказа на основе содержимого корзины пользователя с указанием адреса доставки")]
        public async Task<IActionResult> Checkout([FromBody] CheckoutRequest request, CancellationToken cancellationToken)
        {
            var order = await _cartService.CheckoutAsync(request.UserId, request.ShippingAddress, cancellationToken);
            return Ok(order);
        }

    }
}
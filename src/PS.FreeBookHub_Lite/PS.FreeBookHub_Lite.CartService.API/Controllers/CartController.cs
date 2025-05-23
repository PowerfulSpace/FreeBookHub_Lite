using Microsoft.AspNetCore.Mvc;
using PS.FreeBookHub_Lite.CartService.Application.DTOs;
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
        public async Task<IActionResult> Get(Guid userId)
        {
            var cart = await _cartService.GetCartAsync(userId);
            return Ok(cart);
        }

        [HttpPost("{userId:guid}/books")]
        [SwaggerOperation(Summary = "Добавление товара в корзину", Description = "Добавляет книгу в корзину пользователя")]
        public async Task<IActionResult> AddBook(Guid userId, [FromBody] AddItemRequest request)
        {
            await _cartService.AddItemAsync(userId, request);
            return NoContent();
        }

        [HttpPatch("{userId:guid}/items")]
        [SwaggerOperation(Summary = "Обновление количества товара", Description = "Изменяет количество книги в корзине пользователя")]
        public async Task<IActionResult> UpdateBookQuantity(Guid userId, [FromBody] UpdateItemQuantityRequest request)
        {
            await _cartService.UpdateItemQuantityAsync(userId, request);
            return NoContent();
        }

        [HttpDelete("{userId:guid}/items/{bookId:guid}")]
        [SwaggerOperation(Summary = "Удаление товара из корзины", Description = "Удаляет указанный товар из корзины пользователя")]
        public async Task<IActionResult> RemoveBook(Guid userId, Guid bookId)
        {
            await _cartService.RemoveItemAsync(userId, bookId);
            return NoContent();
        }

        [HttpPut("{userId:guid}")]
        [SwaggerOperation(Summary = "Очистка корзины", Description = "Удаляет все товары из корзины пользователя")]
        public async Task<IActionResult> ClearCart(Guid userId)
        {
            await _cartService.ClearCartAsync(userId);
            return NoContent();
        }

        [HttpPost("checkout")]
        [SwaggerOperation(Summary = "Оформление заказа из корзины", Description = "Выполняет оформление заказа на основе содержимого корзины пользователя с указанием адреса доставки")]
        public async Task<IActionResult> Checkout([FromBody] CheckoutRequest request)
        {
            var order = await _cartService.CheckoutAsync(request.UserId, request.ShippingAddress);
            return Ok(order);
        }

    }
}
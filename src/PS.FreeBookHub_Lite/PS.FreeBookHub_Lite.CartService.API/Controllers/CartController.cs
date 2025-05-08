using Microsoft.AspNetCore.Mvc;
using PS.FreeBookHub_Lite.CartService.Application.DTOs;
using PS.FreeBookHub_Lite.CartService.Application.Services.Interfaces;

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

        [HttpGet("{userId:guid}", Name = "GetCart")]
        public async Task<IActionResult> GetCart(Guid userId)
        {
            var cart = await _cartService.GetCartAsync(userId);
            return Ok(cart);
        }

        [HttpPost("{userId:guid}/items")]
        public async Task<IActionResult> AddItem(Guid userId, [FromBody] AddItemRequest request)
        {
            await _cartService.AddItemAsync(userId, request);
            return NoContent();
        }

        [HttpPut("{userId:guid}/items")]
        public async Task<IActionResult> UpdateItem(Guid userId, [FromBody] UpdateItemQuantityRequest request)
        {
            await _cartService.UpdateItemQuantityAsync(userId, request);
            return NoContent();
        }

        [HttpPatch("{userId:guid}/items/{bookId:guid}")]
        public async Task<IActionResult> RemoveItem(Guid userId, Guid bookId)
        {
            await _cartService.RemoveItemAsync(userId, bookId);
            return NoContent();
        }

        [HttpPut("{userId:guid}", Name = "ClearCart")]
        public async Task<IActionResult> ClearCart(Guid userId)
        {
            await _cartService.ClearCartAsync(userId);
            return NoContent();
        }
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PS.FreeBookHub_Lite.CatalogService.Application.DTOs;
using PS.FreeBookHub_Lite.CatalogService.Application.Services.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace PS.FreeBookHub_Lite.CatalogService.API.Controllers
{
    [Authorize(Policy = "User")]
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly IBookService _bookService;

        public BooksController(IBookService bookService)
        {
            _bookService = bookService;
        }

        [HttpGet(Name = "GetBooksAll")]
        [SwaggerOperation(Summary = "Получение всех книг", Description = "Возвращает список всех доступных книг в каталоге")]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var books = await _bookService.GetAllBooksAsync(cancellationToken);
            return Ok(books);
        }

        [HttpGet("{id:guid}",Name = "GetBookById")]
        [SwaggerOperation(Summary = "Получение книги по идентификатору", Description = "Возвращает детальную информацию о книге по её идентификатору")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var book = await _bookService.GetBookByIdAsync(id, cancellationToken);
            return book is not null ? Ok(book) : NotFound();
        }

        [HttpPost(Name = "CreateBook")]
        [SwaggerOperation(Summary = "Создание новой книги", Description = "Добавляет новую книгу в каталог")]
        public async Task<IActionResult> Create([FromBody] CreateBookRequest request, CancellationToken cancellationToken)
        {
            var result = await _bookService.CreateBookAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id:guid}", Name = "UpdateBook")]
        [SwaggerOperation(Summary = "Обновление информации о книге", Description = "Обновляет информацию о существующей книге в каталоге")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateBookRequest request, CancellationToken cancellationToken)
        {
            var updated = await _bookService.UpdateBookAsync(id, request, cancellationToken);
            return updated ? NoContent() : NotFound();
        }

        [HttpDelete("{id:guid}", Name = "DeleteBook")]
        [SwaggerOperation(Summary = "Удаление книги", Description = "Удаляет книгу из каталога по её идентификатору")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            var deleted = await _bookService.DeleteBookAsync(id, cancellationToken);
            return deleted ? NoContent() : NotFound();
        }

        [HttpGet("{id:guid}/price")]
        [SwaggerOperation(Summary = "Получение цены книги", Description = "Возвращает текущую цену указанной книги")]
        public async Task<IActionResult> GetPrice(Guid id, CancellationToken cancellationToken)
        {
            var price = await _bookService.GetBookPriceAsync(id, cancellationToken);
            return price.HasValue ? Ok(price.Value) : NotFound();
        }
    }
}
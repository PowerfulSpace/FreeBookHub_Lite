using Microsoft.AspNetCore.Mvc;
using PS.FreeBookHub_Lite.CatalogService.Application.DTOs;
using PS.FreeBookHub_Lite.CatalogService.Application.Services.Interfaces;

namespace PS.FreeBookHub_Lite.CatalogService.API.Controllers
{
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
        public async Task<IActionResult> GetAll()
        {
            var books = await _bookService.GetAllBooksAsync();
            return Ok(books);
        }

        [HttpGet("{id:guid}",Name = "GetBookById")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var book = await _bookService.GetBookByIdAsync(id);
            return book is not null ? Ok(book) : NotFound();
        }

        [HttpPost(Name = "CreateBook")]
        public async Task<IActionResult> Create(CreateBookRequest request)
        {
            var result = await _bookService.CreateBookAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id:guid}", Name = "UpdateBook")]
        public async Task<IActionResult> Update(Guid id, CreateBookRequest request)
        {
            var updated = await _bookService.UpdateBookAsync(id, request);
            return updated ? NoContent() : NotFound();
        }

        [HttpDelete("{id:guid}", Name = "DeleteBook")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _bookService.DeleteBookAsync(id);
            return deleted ? NoContent() : NotFound();
        }
    }
}
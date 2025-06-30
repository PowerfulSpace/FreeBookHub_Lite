using Mapster;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PS.FreeBookHub_Lite.CatalogService.Application.CQRS.Commands.CreateBook;
using PS.FreeBookHub_Lite.CatalogService.Application.CQRS.Commands.DeleteBook;
using PS.FreeBookHub_Lite.CatalogService.Application.CQRS.Commands.UpdateBook;
using PS.FreeBookHub_Lite.CatalogService.Application.CQRS.Queries.GetAllBooks;
using PS.FreeBookHub_Lite.CatalogService.Application.CQRS.Queries.GetBookById;
using PS.FreeBookHub_Lite.CatalogService.Application.CQRS.Queries.GetBookPrice;
using PS.FreeBookHub_Lite.CatalogService.Application.DTOs;
using Swashbuckle.AspNetCore.Annotations;

namespace PS.FreeBookHub_Lite.CatalogService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BooksController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [HttpGet(Name = "GetBooksAll")]
        [SwaggerOperation(Summary = "Получение всех книг", Description = "Возвращает список всех доступных книг в каталоге")]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var query = new GetAllBooksQuery();
            var books = await _mediator.Send(query, cancellationToken);

            return Ok(books);
        }

        [HttpGet("{id:guid}",Name = "GetBookById")]
        [SwaggerOperation(Summary = "Получение книги по идентификатору", Description = "Возвращает детальную информацию о книге по её идентификатору")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var query = new GetBookByIdQuery(id);
            var book = await _mediator.Send(query, cancellationToken);

            return book is not null ? Ok(book) : NotFound();
        }

        [HttpGet("{id:guid}/price")]
        [SwaggerOperation(Summary = "Получение цены книги", Description = "Возвращает текущую цену указанной книги")]
        public async Task<IActionResult> GetPrice(Guid id, CancellationToken cancellationToken)
        {
            var query = new GetBookPriceQuery(id);
            var price = await _mediator.Send(query, cancellationToken);

            return price.HasValue ? Ok(price.Value) : NotFound();
        }

        [Authorize(Policy = "Moderator")]
        [HttpPost(Name = "CreateBook")]
        [SwaggerOperation(Summary = "Создание новой книги", Description = "Добавляет новую книгу в каталог")]
        public async Task<IActionResult> Create([FromBody] CreateBookRequest request, CancellationToken cancellationToken)
        {
            var command = request.Adapt<CreateBookCommand>();
            var result = await _mediator.Send(command, cancellationToken);

            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [Authorize(Policy = "Moderator")]
        [HttpPut("{id:guid}", Name = "UpdateBook")]
        [SwaggerOperation(Summary = "Обновление информации о книге", Description = "Обновляет информацию о существующей книге в каталоге")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateBookRequest request, CancellationToken cancellationToken)
        {
            var command = request.Adapt<UpdateBookCommand>();
            var updated = await _mediator.Send(command, cancellationToken);

            return updated ? NoContent() : NotFound();
        }

        [Authorize(Policy = "Moderator")]
        [HttpDelete("{id:guid}", Name = "DeleteBook")]
        [SwaggerOperation(Summary = "Удаление книги", Description = "Удаляет книгу из каталога по её идентификатору")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            var command = new DeleteBookCommand() { Id = id};
            var deleted = await _mediator.Send(command, cancellationToken);

            return deleted ? NoContent() : NotFound();
        }
    }
}
using MediatR;
using PS.CatalogService.Application.DTOs;

namespace PS.CatalogService.Application.CQRS.Queries.GetAllBooks
{
    public class GetAllBooksQuery : IRequest<IEnumerable<BookResponse>>
    {
    }
}

using MediatR;
using PS.FreeBookHub_Lite.CatalogService.Application.DTOs;

namespace PS.FreeBookHub_Lite.CatalogService.Application.CQRS.Queries.GetAllBooks
{
    public class GetAllBooksQuery : IRequest<IEnumerable<BookResponse>>
    {
    }
}

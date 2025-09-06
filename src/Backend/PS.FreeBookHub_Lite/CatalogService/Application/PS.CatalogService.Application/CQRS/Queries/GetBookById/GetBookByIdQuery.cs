using MediatR;
using PS.FreeBookHub_Lite.CatalogService.Application.DTOs;

namespace PS.FreeBookHub_Lite.CatalogService.Application.CQRS.Queries.GetBookById
{
    public class GetBookByIdQuery : IRequest<BookResponse>
    {
        public Guid Id { get; set; }

        public GetBookByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}

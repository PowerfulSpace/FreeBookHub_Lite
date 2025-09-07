using MediatR;
using PS.CatalogService.Application.DTOs;

namespace PS.CatalogService.Application.CQRS.Queries.GetBookById
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

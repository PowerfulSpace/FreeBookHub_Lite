using MediatR;

namespace PS.FreeBookHub_Lite.CatalogService.Application.CQRS.Queries.GetBookPrice
{
    public class GetBookPriceQuery : IRequest<decimal?>
    {
        public Guid Id { get; set; }

        public GetBookPriceQuery(Guid id)
        {
            Id = id;
        }
    }
}

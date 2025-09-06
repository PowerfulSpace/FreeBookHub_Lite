using MediatR;

namespace PS.FreeBookHub_Lite.CatalogService.Application.CQRS.Commands.DeleteBook
{
    public class DeleteBookCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
    }
}

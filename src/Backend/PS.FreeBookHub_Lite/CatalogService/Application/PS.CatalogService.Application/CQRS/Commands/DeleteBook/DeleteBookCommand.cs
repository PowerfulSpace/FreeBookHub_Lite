using MediatR;

namespace PS.CatalogService.Application.CQRS.Commands.DeleteBook
{
    public class DeleteBookCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
    }
}

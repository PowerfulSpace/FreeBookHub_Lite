using MediatR;
using PS.FreeBookHub_Lite.CatalogService.Application.DTOs;

namespace PS.FreeBookHub_Lite.CatalogService.Application.CQRS.Commands.UpdateBook
{
    public class UpdateBookCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
        public UpdateBookRequest Request { get; set; } = default!;
    }
}

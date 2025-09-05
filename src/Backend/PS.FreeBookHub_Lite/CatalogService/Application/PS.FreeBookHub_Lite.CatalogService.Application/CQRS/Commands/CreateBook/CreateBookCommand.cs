using MediatR;
using PS.FreeBookHub_Lite.CatalogService.Application.DTOs;

namespace PS.FreeBookHub_Lite.CatalogService.Application.CQRS.Commands.CreateBook
{
    public class CreateBookCommand : IRequest<BookResponse>
    {
        public string Title { get; set; } = default!;
        public string Author { get; set; } = default!;
        public string Description { get; set; } = default!;
        public string ISBN { get; set; } = default!;
        public decimal Price { get; set; }
        public DateTime PublishedAt { get; set; }
        public string CoverImageUrl { get; set; } = default!;
    }
}

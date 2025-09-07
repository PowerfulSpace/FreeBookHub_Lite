using MediatR;
using PS.CatalogService.Application.DTOs;

namespace PS.CatalogService.Application.CQRS.Commands.CreateBook
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

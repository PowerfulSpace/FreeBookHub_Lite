﻿namespace PS.FreeBookHub_Lite.CatalogService.Application.DTOs
{
    public class UpdateBookRequest
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

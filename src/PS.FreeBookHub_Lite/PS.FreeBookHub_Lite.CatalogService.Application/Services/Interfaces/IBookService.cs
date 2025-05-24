using PS.FreeBookHub_Lite.CatalogService.Application.DTOs;

namespace PS.FreeBookHub_Lite.CatalogService.Application.Services.Interfaces
{
    public interface IBookService
    {
        Task<IEnumerable<BookResponse>> GetAllBooksAsync(CancellationToken cancellationToken);
        Task<BookResponse?> GetBookByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<BookResponse> CreateBookAsync(CreateBookRequest request, CancellationToken cancellationToken);
        Task<bool> DeleteBookAsync(Guid id, CancellationToken cancellationToken);
        Task<bool> UpdateBookAsync(Guid id, UpdateBookRequest request, CancellationToken cancellationToken);
        Task<decimal?> GetBookPriceAsync(Guid id, CancellationToken cancellationToken);
    }
}

using PS.FreeBookHub_Lite.CatalogService.Application.DTOs;

namespace PS.FreeBookHub_Lite.CatalogService.Application.Services.Interfaces
{
    public interface IBookService
    {
        Task<IEnumerable<BookResponse>> GetAllBooksAsync();
        Task<BookResponse?> GetBookByIdAsync(Guid id);
        Task<BookResponse> CreateBookAsync(CreateBookRequest request);
        Task<bool> DeleteBookAsync(Guid id);
        Task<bool> UpdateBookAsync(Guid id, UpdateBookRequest request);
        Task<decimal?> GetBookPriceAsync(Guid id);
    }
}

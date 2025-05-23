using PS.FreeBookHub_Lite.CatalogService.Application.DTOs;

namespace PS.FreeBookHub_Lite.CatalogService.Application.Services.Interfaces
{
    public interface IBookService
    {
        Task<IEnumerable<BookDto>> GetAllBooksAsync();
        Task<BookDto?> GetBookByIdAsync(Guid id);
        Task<BookDto> CreateBookAsync(CreateBookRequest request);
        Task<bool> DeleteBookAsync(Guid id);
        Task<bool> UpdateBookAsync(Guid id, UpdateBookRequest request);
        Task<decimal?> GetBookPriceAsync(Guid id);
    }
}

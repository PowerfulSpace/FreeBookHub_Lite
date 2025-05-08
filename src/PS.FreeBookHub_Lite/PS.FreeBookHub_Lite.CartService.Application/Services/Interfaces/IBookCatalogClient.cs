namespace PS.FreeBookHub_Lite.CartService.Application.Services.Interfaces
{
    public interface IBookCatalogClient
    {
        Task<decimal?> GetBookPriceAsync(Guid bookId);
    }
}

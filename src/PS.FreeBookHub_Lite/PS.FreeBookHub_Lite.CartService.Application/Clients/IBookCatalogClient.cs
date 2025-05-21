namespace PS.FreeBookHub_Lite.CartService.Application.Clients
{
    public interface IBookCatalogClient
    {
        Task<decimal?> GetBookPriceAsync(Guid bookId);
    }
}

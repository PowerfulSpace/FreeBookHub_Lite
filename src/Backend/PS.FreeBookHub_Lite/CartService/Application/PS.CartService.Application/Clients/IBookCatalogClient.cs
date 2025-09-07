namespace PS.CartService.Application.Clients
{
    public interface IBookCatalogClient
    {
        Task<decimal?> GetBookPriceAsync(Guid bookId, CancellationToken cancellationToken);
    }
}

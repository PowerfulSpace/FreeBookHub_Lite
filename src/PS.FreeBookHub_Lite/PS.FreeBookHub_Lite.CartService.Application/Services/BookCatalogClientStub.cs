using PS.FreeBookHub_Lite.CartService.Application.Services.Interfaces;

namespace PS.FreeBookHub_Lite.CartService.Application.Services
{
    public class BookCatalogClientStub : IBookCatalogClient
    {
        public Task<decimal?> GetBookPriceAsync(Guid bookId)
        {
            // Временно возвращаем фиксированную цену
            return Task.FromResult<decimal?>(9.99m);
        }
    }
}

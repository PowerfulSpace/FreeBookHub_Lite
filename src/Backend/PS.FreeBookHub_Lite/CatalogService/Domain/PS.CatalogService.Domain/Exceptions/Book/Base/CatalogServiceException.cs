namespace PS.FreeBookHub_Lite.CatalogService.Domain.Exceptions.Book.Base
{
    public abstract class CatalogServiceException : Exception
    {
        protected CatalogServiceException(string message) : base(message) { }
    }
}

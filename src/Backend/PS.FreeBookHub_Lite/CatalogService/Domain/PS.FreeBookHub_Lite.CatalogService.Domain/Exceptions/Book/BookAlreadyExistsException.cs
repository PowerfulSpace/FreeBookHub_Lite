using PS.FreeBookHub_Lite.CatalogService.Domain.Exceptions.Book.Base;

namespace PS.FreeBookHub_Lite.CatalogService.Domain.Exceptions.Book
{
    public class BookAlreadyExistsException : CatalogServiceException
    {
        public string Title { get; }

        public BookAlreadyExistsException(string title)
            : base($"Book already exists (Title: {title})")
        {
            Title = title;
        }
    }
}

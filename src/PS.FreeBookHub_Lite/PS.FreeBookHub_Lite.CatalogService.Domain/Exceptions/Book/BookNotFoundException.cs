using PS.FreeBookHub_Lite.CatalogService.Domain.Exceptions.Book.Base;

namespace PS.FreeBookHub_Lite.CatalogService.Domain.Exceptions.Book
{
    public class BookNotFoundException : CatalogServiceException
    {
        public Guid BookId { get; }

        public BookNotFoundException(Guid bookId)
            : base($"Book not found (ID: {bookId})")
        {
            BookId = bookId;
        }
    }
}

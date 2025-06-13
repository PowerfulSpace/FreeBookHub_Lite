using PS.FreeBookHub_Lite.CartService.Domain.Exceptions.Base;

namespace PS.FreeBookHub_Lite.CartService.Domain.Exceptions
{
    public class BookNotFoundException : CartServiceException
    {
        public Guid BookId { get; }

        public BookNotFoundException(Guid bookId)
             : base($"Book with ID {bookId} not found or price unavailable")
        {
            BookId = bookId;
        }
    }
}

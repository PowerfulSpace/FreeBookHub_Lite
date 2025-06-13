using PS.FreeBookHub_Lite.CartService.Domain.Exceptions.Base;

namespace PS.FreeBookHub_Lite.CartService.Domain.Exceptions
{
    public class BookNotFoundException : CartServiceException
    {
        public Guid BookId { get; }

        public BookNotFoundException(Guid bookId)
            : base($"Книга с ID {bookId} не найдена или цена недоступна")
        {
            BookId = bookId;
        }
    }
}

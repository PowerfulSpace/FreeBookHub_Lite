namespace PS.FreeBookHub_Lite.CatalogService.Common
{
    public static class LoggerMessages
    {
        // BookService
        public const string GetAllBooksStarted = "Fetching all books";
        public const string GetAllBooksSuccess = "Retrieved {Count} books";

        public const string GetBookByIdStarted = "Fetching book by ID: {BookId}";
        public const string GetBookByIdNotFound = "Book not found — ID: {BookId}";
        public const string GetBookByIdSuccess = "Book retrieved — ID: {BookId}";

        public const string CreateBookStarted = "Creating book — Title: {Title}";
        public const string CreateBookAlreadyExists = "Book already exists — Title: {Title}";
        public const string CreateBookSuccess = "Book created — ID: {BookId}, Title: {Title}";

        public const string DeleteBookStarted = "Deleting book — ID: {BookId}";
        public const string DeleteBookNotFound = "Book not found for deletion — ID: {BookId}";
        public const string DeleteBookSuccess = "Book deleted — ID: {BookId}";

        public const string UpdateBookStarted = "Updating book — ID: {BookId}";
        public const string UpdateBookNotFound = "Book not found for update — ID: {BookId}";
        public const string UpdateBookSuccess = "Book updated — ID: {BookId}";

        public const string GetBookPriceStarted = "Fetching price for book — ID: {BookId}";
        public const string GetBookPriceNotFound = "Book not found for price check — ID: {BookId}";
        public const string GetBookPriceSuccess = "Price retrieved — ID: {BookId}, Price: {Price}";
    }
}

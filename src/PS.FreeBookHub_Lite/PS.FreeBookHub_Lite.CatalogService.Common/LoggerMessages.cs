namespace PS.FreeBookHub_Lite.CatalogService.Common
{
    public static class LoggerMessages
    {
        // ExceptionHandlingMiddleware
        //                  --- Book Errors
        public const string GetBookByIdNotFound = "Book not found — ID: {BookId} | Method: {Method} | Path: {Path}";
        public const string CreateBookAlreadyExists = "Book already exists — Title: {Title} | Method: {Method} | Path: {Path}";

        //                  --- General Error Handling
        public const string UnhandledException = "Unhandled exception — Message: {Message} | Method: {Method} | Path: {Path}";


        // BookService
        //                  --- GetAllBooksAsync
        public const string GetAllBooksStarted = "Fetching all books";
        public const string GetAllBooksSuccess = "Retrieved {Count} books";

        //                  --- GetBookByIdAsync
        public const string GetBookByIdStarted = "Fetching book by ID: {BookId}";
        public const string GetBookByIdSuccess = "Book retrieved — ID: {BookId}";

        //                  --- CreateBookAsync
        public const string CreateBookStarted = "Creating book — Title: {Title}";
        public const string CreateBookSuccess = "Book created — ID: {BookId}, Title: {Title}";

        //                  --- DeleteBookAsync
        public const string DeleteBookStarted = "Deleting book — ID: {BookId}";
        public const string DeleteBookSuccess = "Book deleted — ID: {BookId}";

        //                  --- UpdateBookAsync
        public const string UpdateBookStarted = "Updating book — ID: {BookId}";
        public const string UpdateBookSuccess = "Book updated — ID: {BookId}";

        //                  --- GetBookPriceAsync
        public const string GetBookPriceStarted = "Fetching price for book — ID: {BookId}";
        public const string GetBookPriceSuccess = "Price retrieved — ID: {BookId}, Price: {Price}";
    }
}

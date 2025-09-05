namespace PS.FreeBookHub_Lite.CatalogService.Common.Logging
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
        public const string GetAllBooksStarted = "[BOOK] GET_ALL started";
        public const string GetAllBooksSuccess = "[BOOK] GET_ALL success | Count:{Count}";

        //                  --- GetBookByIdAsync
        public const string GetBookByIdStarted = "[BOOK] GET_BY_ID started | BookId:{BookId}";
        public const string GetBookByIdSuccess = "[BOOK] GET_BY_ID success | BookId:{BookId}";

        //                  --- CreateBookAsync
        public const string CreateBookStarted = "[BOOK] CREATE started | Title:{Title}";
        public const string CreateBookSuccess = "[BOOK] CREATE success | BookId:{BookId} | Title:{Title}";

        //                  --- DeleteBookAsync
        public const string DeleteBookStarted = "[BOOK] DELETE started | BookId:{BookId}";
        public const string DeleteBookSuccess = "[BOOK] DELETE success | BookId:{BookId}";

        //                  --- UpdateBookAsync
        public const string UpdateBookStarted = "[BOOK] UPDATE started | BookId:{BookId}";
        public const string UpdateBookSuccess = "[BOOK] UPDATE success | BookId:{BookId}";

        //                  --- GetBookPriceAsync
        public const string GetBookPriceStarted = "[BOOK] GET_PRICE started | BookId:{BookId}";
        public const string GetBookPriceSuccess = "[BOOK] GET_PRICE success | BookId:{BookId} | Price:{Price}";
    }
}
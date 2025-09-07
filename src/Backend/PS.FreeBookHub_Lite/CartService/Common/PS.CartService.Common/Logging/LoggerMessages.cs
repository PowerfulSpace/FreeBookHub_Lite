namespace PS.CartService.Common.Logging
{
    public static class LoggerMessages
    {
        // ExceptionHandlingMiddleware
        //                  --- Cart Errors
        public const string BookNotFound = "Book not found | BookId: {BookId} | Method: {Method} | Path: {Path}";
        public const string CartNotFound = "Cart not found | UserId: {UserId} | Method: {Method} | Path: {Path}";
        public const string EmptyCart = "Empty cart | UserId: {UserId} | Method: {Method} | Path: {Path}";
        public const string InvalidCartItemQuantity = "Invalid quantity in cart item | Quantity: {Quantity} | Method: {Method} | Path: {Path}";
        public const string CartItemNotFound = "Cart item not found | UserId: {UserId}, BookId: {BookId} | Method: {Method} | Path: {Path}";
        public const string InvalidUserIdentifier = "Invalid user identifier — InvalidId: {InvalidId} | Method: {Method} | Path: {Path}";


        //                  --- OrderServiceClient Errors
        public const string CreateOrderFailed = "Order creation failed | UserId: {UserId}";

        //                  --- General Error Handling
        public const string UnhandledException = "Unhandled exception — Message: {Message} | Method: {Method} | Path: {Path}";



        // CartBookService
        //                  --- GetCartAsync
        public const string GetCartStarted = "[Cart] GET started | UserId:{UserId}";
        public const string GetCartSuccess = "[Cart] GET success | UserId:{UserId}";

        //                  --- AddItemAsync
        public const string AddItemStarted = "[Cart] ADD started | UserId:{UserId} | BookId:{BookId} | Qty:{Quantity}";
        public const string AddItemSuccess = "[Cart] ADD success | UserId:{UserId} | BookId:{BookId}";
        public const string CartCreated = "[Cart] CREATED | UserId:{UserId}";

        //                  --- UpdateItemQuantityAsync
        public const string UpdateQuantityStarted = "[Cart] UPDATE started | UserId:{UserId} | BookId:{BookId} | Qty:{Quantity}";
        public const string UpdateQuantitySuccess = "[Cart] UPDATE success | UserId:{UserId} | BookId:{BookId}";

        //                  --- RemoveItemAsync
        public const string RemoveItemStarted = "[Cart] REMOVE started | UserId:{UserId} | BookId:{BookId}";
        public const string RemoveItemSuccess = "[Cart] REMOVE success | UserId:{UserId} | BookId:{BookId}";

        //                  --- ClearCartAsync
        public const string ClearCartStarted = "[Cart] CLEAR started | UserId:{UserId}";
        public const string ClearCartSuccess = "[Cart] CLEAR success | UserId:{UserId}";

        //                  --- CheckoutAsync
        public const string CheckoutStarted = "[Cart] CHECKOUT started | UserId:{UserId}";
        public const string CheckoutSuccess = "[Cart] CHECKOUT success | UserId:{UserId} | OrderId:{OrderId}";

        // BookCatalogClient
        public const string GetBookPriceStarted = "[Catalog] GET_PRICE started | BookId:{BookId}";
        public const string GetBookPriceSuccess = "[Catalog] GET_PRICE success | BookId:{BookId}";

        // OrderServiceClient
        public const string CreateOrderStarted = "[Order] CREATE started | UserId:{UserId}";
        public const string CreateOrderSuccess = "[Order] CREATE success | UserId:{UserId} | OrderId:{OrderId}";

    }
}
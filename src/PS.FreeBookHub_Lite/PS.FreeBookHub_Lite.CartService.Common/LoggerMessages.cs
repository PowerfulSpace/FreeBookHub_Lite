namespace PS.FreeBookHub_Lite.CartService.Common
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
        public const string GetCartStarted = "Cart retrieval started | UserId: {UserId}";
        public const string GetCartSuccess = "Cart successfully retrieved | UserId: {UserId}";

        //                  --- AddItemAsync
        public const string AddItemStarted = "Adding item to cart | UserId: {UserId}, BookId: {BookId}, Quantity: {Quantity}";
        public const string AddItemSuccess = "Item successfully added to cart | UserId: {UserId}, BookId: {BookId}";
        public const string CartCreated = "New cart created | UserId: {UserId}";

        //                  --- UpdateItemQuantityAsync
        public const string UpdateQuantityStarted = "Updating item quantity | UserId: {UserId}, BookId: {BookId}, NewQuantity: {Quantity}";
        public const string UpdateQuantitySuccess = "Item quantity updated | UserId: {UserId}, BookId: {BookId}";

        //                  --- RemoveItemAsync
        public const string RemoveItemStarted = "Removing item from cart | UserId: {UserId}, BookId: {BookId}";
        public const string RemoveItemSuccess = "Item removed from cart | UserId: {UserId}, BookId: {BookId}";

        //                  --- ClearCartAsync
        public const string ClearCartStarted = "Clearing cart | UserId: {UserId}";
        public const string ClearCartSuccess = "Cart cleared | UserId: {UserId}";

        //                  --- CheckoutAsync
        public const string CheckoutStarted = "Checkout started | UserId: {UserId}";
        public const string CheckoutSuccess = "Checkout successful | UserId: {UserId}, OrderId: {OrderId}";


        // BookCatalogClient
        public const string GetBookPriceStarted = "Getting book price | BookId: {BookId}";
        public const string GetBookPriceSuccess = "Book price received | BookId: {BookId}";

        // OrderServiceClient
        public const string CreateOrderStarted = "Creating order | UserId: {UserId}";
        public const string CreateOrderSuccess = "Order created | UserId: {UserId}, OrderId: {OrderId}";
        
    }
}

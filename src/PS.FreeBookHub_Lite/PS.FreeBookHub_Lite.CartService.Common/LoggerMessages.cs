namespace PS.FreeBookHub_Lite.CartService.Common
{
    public static class LoggerMessages
    {
        // Cart operations
        public const string GetCartStarted = "Cart retrieval started | UserId: {UserId}";
        public const string GetCartSuccess = "Cart successfully retrieved | UserId: {UserId}";

        public const string AddItemStarted = "Adding item to cart | UserId: {UserId}, BookId: {BookId}, Quantity: {Quantity}";
        public const string AddItemSuccess = "Item successfully added to cart | UserId: {UserId}, BookId: {BookId}";
        public const string CartCreated = "New cart created | UserId: {UserId}";

        public const string UpdateQuantityStarted = "Updating item quantity | UserId: {UserId}, BookId: {BookId}, NewQuantity: {Quantity}";
        public const string UpdateQuantitySuccess = "Item quantity updated | UserId: {UserId}, BookId: {BookId}";

        public const string RemoveItemStarted = "Removing item from cart | UserId: {UserId}, BookId: {BookId}";
        public const string RemoveItemSuccess = "Item removed from cart | UserId: {UserId}, BookId: {BookId}";

        public const string ClearCartStarted = "Clearing cart | UserId: {UserId}";
        public const string ClearCartSuccess = "Cart cleared | UserId: {UserId}";

        public const string CheckoutStarted = "Checkout started | UserId: {UserId}";
        public const string CheckoutSuccess = "Checkout successful | UserId: {UserId}, OrderId: {OrderId}";

        // Warnings
        public const string BookNotFound = "Book not found | BookId: {BookId}";
        public const string ItemNotFoundInCart = "Item not found in cart | UserId: {UserId}, BookId: {BookId}";
        public const string EmptyCartCheckout = "Attempt to checkout an empty cart | UserId: {UserId}";
    }
}

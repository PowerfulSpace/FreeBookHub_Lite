namespace PS.FreeBookHub_Lite.CartService.Domain.Entities
{
    public class CartItem
    {
        public Guid BookId { get; private set; }
        public int Quantity { get; set; }

        public CartItem(Guid bookId, int quantity)
        {
            BookId = bookId;
            Quantity = quantity;
        }
    }
}
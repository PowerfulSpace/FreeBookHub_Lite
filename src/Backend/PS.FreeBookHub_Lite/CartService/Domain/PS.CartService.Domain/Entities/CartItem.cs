using PS.CartService.Domain.Exceptions.Cart;

namespace PS.CartService.Domain.Entities
{
    public class CartItem
    {
        public Guid BookId { get; private set; }
        public int Quantity { get; private set; }

        public decimal UnitPrice { get; private set; }
        public decimal TotalPrice => UnitPrice * Quantity;

        public Guid CartId { get; private set; }
        public Cart? Cart { get; private set; }


        protected CartItem() { }

        public CartItem(Guid bookId, int quantity, decimal unitPrice)
        {
            BookId = bookId;
            Quantity = quantity;
            UnitPrice = unitPrice;
        }

        public void UpdateQuantity(int quantity)
        {
            if (quantity <= 0)
                throw new InvalidCartItemQuantityException(quantity);

            Quantity = quantity;
        }
    }
}
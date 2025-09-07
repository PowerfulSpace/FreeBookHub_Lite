using PS.CartService.Domain.Exceptions.Cart;

namespace PS.CartService.Domain.Entities
{
    public class Cart
    {
        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }

        private readonly List<CartItem> _items = new();
        public IReadOnlyCollection<CartItem> Items => _items.AsReadOnly();

        protected Cart() { }
        public Cart(Guid userId)
        {
            Id = Guid.NewGuid();
            UserId = userId;
        }

        public void AddItem(Guid bookId, int quantity, decimal unitPrice)
        {
            if (quantity <= 0)
                throw new InvalidCartItemQuantityException(quantity);

            var existingItem = _items.FirstOrDefault(i => i.BookId == bookId);
            if (existingItem is not null)
            {
                existingItem.UpdateQuantity(existingItem.Quantity + quantity);
            }
            else
            {
                _items.Add(new CartItem(bookId, quantity, unitPrice));
            }
        }

        public void RemoveItem(Guid bookId)
        {
            _items.RemoveAll(i => i.BookId == bookId);
        }

        public void UpdateQuantity(Guid bookId, int quantity)
        {
            var item = _items.FirstOrDefault(i => i.BookId == bookId);

            if (item is null)
                throw new CartItemNotFoundException(UserId, bookId);

            item?.UpdateQuantity(quantity);
        }

        public decimal TotalPrice => _items.Sum(i => i.TotalPrice);

        public void Clear()
        {
            _items.Clear();
        }
    }
}

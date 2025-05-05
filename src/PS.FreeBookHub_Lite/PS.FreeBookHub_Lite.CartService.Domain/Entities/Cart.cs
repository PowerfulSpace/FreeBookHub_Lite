namespace PS.FreeBookHub_Lite.CartService.Domain.Entities
{
    public class Cart
    {
        public Guid UserId { get; set; }

        public List<CartItem> Items { get; set; } = new();

        public void AddItem(Guid bookId, int quantity)
        {
            var item = Items.FirstOrDefault(i => i.BookId == bookId);
            if (item is not null)
            {
                item.Quantity += quantity;
            }
            else
            {
                Items.Add(new CartItem(bookId, quantity));
            }
        }

        public void RemoveItem(Guid bookId)
        {
            Items.RemoveAll(i => i.BookId == bookId);
        }

        public void UpdateQuantity(Guid bookId, int quantity)
        {
            var item = Items.FirstOrDefault(i => i.BookId == bookId);
            if (item is not null)
            {
                item.Quantity = quantity;
            }
        }

        public void Clear()
        {
            Items.Clear();
        }
    }
}

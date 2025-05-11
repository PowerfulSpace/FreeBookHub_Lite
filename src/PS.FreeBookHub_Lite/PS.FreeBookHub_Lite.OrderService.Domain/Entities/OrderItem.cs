namespace PS.FreeBookHub_Lite.OrderService.Domain.Entities
{
    public class OrderItem
    {
        public Guid BookId { get; private set; }
        public int Quantity { get; private set; }

        public decimal UnitPrice { get; private set; }
        public decimal TotalPrice => UnitPrice * Quantity;

        public Guid OrderId { get; set; }
        public Order? Order { get; set; }


        protected OrderItem()
        {   
        }

        public OrderItem(Guid bookId, decimal price, int quantity)
        {
            BookId = bookId;
            Quantity = quantity;
            UnitPrice = price;
        }

        public void UpdateQuantity(int newQuantity)
        {
            if (newQuantity <= 0)
                throw new ArgumentException("Quantity must be positive.");

            Quantity = newQuantity;
        }
    }
}

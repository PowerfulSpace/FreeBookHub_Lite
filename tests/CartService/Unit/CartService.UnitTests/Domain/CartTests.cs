using PS.FreeBookHub_Lite.CartService.Domain.Entities;
using PS.FreeBookHub_Lite.CartService.Domain.Exceptions.Cart;

namespace CartService.UnitTests.Domain
{
    public class CartTests
    {
        [Fact]
        public void CreateCart_ShouldInitializeCorrectly()
        {
            //Arrange
            var userId = Guid.NewGuid();

            // Act
            var cart = new Cart(userId);

            // Assert
            Assert.Equal(userId, cart.UserId);
            Assert.Empty(cart.Items);
            Assert.NotEqual(Guid.Empty, cart.Id);
        }

        [Fact]
        public void AddItem_ShouldAddNewItem_WhenBookNotInCart()
        {
            //Arrange
            var cart = new Cart(Guid.NewGuid());
            var bookId = Guid.NewGuid();

            // Act
            cart.AddItem(bookId, 2, 10m);

            // Assert
            Assert.Single(cart.Items);
            var item = cart.Items.First();
            Assert.Equal(bookId, item.BookId);
            Assert.Equal(2, item.Quantity);
            Assert.Equal(20m, item.TotalPrice);
        }

        [Fact]
        public void AddItem_ShouldIncreaseQuantity_WhenBookAlreadyInCart()
        {
            //Arrange
            var cart = new Cart(Guid.NewGuid());
            var bookId = Guid.NewGuid();

            // Act
            cart.AddItem(bookId, 2, 10m);
            cart.AddItem(bookId, 3, 10m);

            // Assert
            var item = cart.Items.First();
            Assert.Equal(5, item.Quantity);
            Assert.Equal(50m, item.TotalPrice);
        }

        [Fact]
        public void AddItem_WithNonPositiveQuantity_ShouldThrow()
        {
            // Act
            var cart = new Cart(Guid.NewGuid());

            // Assert
            var ex = Assert.Throws<InvalidCartItemQuantityException>(
                () => cart.AddItem(Guid.NewGuid(), 0, 10m)
            );
            Assert.Equal(0, ex.Quantity);
        }

        [Fact]
        public void RemoveItem_ShouldRemoveOnlySpecifiedBook()
        {
            //Arrange
            var cart = new Cart(Guid.NewGuid());
            var book1 = Guid.NewGuid();
            var book2 = Guid.NewGuid();

            // Act
            cart.AddItem(book1, 1, 10m);
            cart.AddItem(book2, 1, 20m);

            cart.RemoveItem(book1);

            // Assert
            Assert.Single(cart.Items);
            Assert.Equal(book2, cart.Items.First().BookId);
        }

        [Fact]
        public void UpdateQuantity_ShouldUpdate_WhenItemExists()
        {
            //Arrange
            var cart = new Cart(Guid.NewGuid());
            var bookId = Guid.NewGuid();

            // Act
            cart.AddItem(bookId, 2, 10m);
            cart.UpdateQuantity(bookId, 5);

            // Assert
            var item = cart.Items.First();
            Assert.Equal(5, item.Quantity);
            Assert.Equal(50m, item.TotalPrice);
        }

        [Fact]
        public void UpdateQuantity_WhenItemDoesNotExist_ShouldThrow()
        {
            //Arrange
            var userId = Guid.NewGuid();
            var cart = new Cart(userId);
            var bookId = Guid.NewGuid();

            // Act
            var ex = Assert.Throws<CartItemNotFoundException>(
                () => cart.UpdateQuantity(bookId, 3)
            );

            // Assert
            Assert.Equal(userId, ex.UserId);
            Assert.Equal(bookId, ex.BookId);
        }

        [Fact]
        public void TotalPrice_ShouldReturnSumOfAllItems()
        {
            //Arrange
            var cart = new Cart(Guid.NewGuid());

            // Act
            cart.AddItem(Guid.NewGuid(), 2, 10m); // 20
            cart.AddItem(Guid.NewGuid(), 3, 5m);  // 15

            // Assert
            Assert.Equal(35m, cart.TotalPrice);
        }

        [Fact]
        public void Clear_ShouldRemoveAllItems()
        {
            var cart = new Cart(Guid.NewGuid());
            cart.AddItem(Guid.NewGuid(), 1, 10m);

            cart.Clear();

            Assert.Empty(cart.Items);
            Assert.Equal(0m, cart.TotalPrice);
        }
    }
}

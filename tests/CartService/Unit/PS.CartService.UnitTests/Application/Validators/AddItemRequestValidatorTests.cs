using FluentValidation.TestHelper;
using PS.CartService.Application.DTOs.Cart;
using PS.CartService.Application.Validators;

namespace PS.CartService.UnitTests.Application.Validators
{
    public class AddItemRequestValidatorTests
    {
        private readonly AddItemRequestValidator _validator = new();

        [Fact]
        public void Validator_ShouldHaveError_WhenBookIdIsEmpty()
        {
            // Arrange
            var model = new AddItemRequest
            {
                BookId = Guid.Empty,
                Quantity = 5
            };
            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.BookId);
        }
    }
}

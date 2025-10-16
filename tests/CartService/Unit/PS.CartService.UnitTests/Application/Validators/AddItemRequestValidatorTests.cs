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

        [Fact]
        public void Validator_ShouldHaveError_WhenQuantityIsZero()
        {
            // Arrange
            var model = new AddItemRequest
            {
                BookId = Guid.NewGuid(),
                Quantity = 0
            };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Quantity)
                  .WithErrorMessage("Quantity must be between 1 and 1000.");
        }

        [Fact]
        public void Validator_ShouldHaveError_WhenQuantityExceedsLimit()
        {
            // Arrange
            var model = new AddItemRequest
            {
                BookId = Guid.NewGuid(),
                Quantity = 5000
            };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Quantity)
                  .WithErrorMessage("Quantity must be between 1 and 1000.");
        }

        [Fact]
        public void Validator_ShouldPass_ForValidRequest()
        {
            // Arrange
            var model = new AddItemRequest
            {
                BookId = Guid.NewGuid(),
                Quantity = 10
            };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}

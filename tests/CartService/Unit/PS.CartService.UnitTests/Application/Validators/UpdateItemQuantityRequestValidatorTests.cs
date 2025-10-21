using FluentValidation.TestHelper;
using PS.CartService.Application.DTOs.Cart;
using PS.CartService.Application.Validators;
using PS.CartService.Domain.Entities;

namespace PS.CartService.UnitTests.Application.Validators
{
    public class UpdateItemQuantityRequestValidatorTests
    {
        private readonly UpdateItemQuantityRequestValidator _validator = new();

        [Fact]
        public void Validator_ShouldHaveError_WhenBookIdIsEmpty()
        {
            // Arrange
            var model = new UpdateItemQuantityRequest
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
        public void Validator_ShouldHaveError_WhenQuantityIsNegative()
        {
            // Arrange
            var model = new UpdateItemQuantityRequest
            {
                BookId = Guid.NewGuid(),
                Quantity = -1
            };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Quantity);
        }

        [Fact]
        public void Validator_ShouldHaveError_WhenQuantityExceeds1000()
        {
            // Arrange
            var model = new UpdateItemQuantityRequest
            {
                BookId = Guid.NewGuid(),
                Quantity = 2000
            };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Quantity);
        }

        [Fact]
        public void Validator_ShouldPass_ForValidRequest()
        {
            // Arrange
            var model = new UpdateItemQuantityRequest
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

using FluentValidation.TestHelper;
using PS.CartService.Application.CQRS.Commands.UpdateItemQuantity;

namespace PS.CartService.UnitTests.Application.CQRS.Commands.UpdateltemQuantity
{
    public class UpdateItemQuantityCommandValidatorTests
    {
        private readonly UpdateItemQuantityCommandValidator _validator = new();

        [Fact]
        public void Validator_ShouldHaveError_WhenUserIdIsEmpty()
        {
            // Arrange
            var command = new UpdateItemQuantityCommand(Guid.Empty, Guid.NewGuid(), 5);

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.UserId)
                  .WithErrorMessage("UserId is required.");
        }


        [Fact]
        public void Validator_ShouldHaveError_WhenBookIdIsEmpty()
        {
            // Arrange
            var command = new UpdateItemQuantityCommand(Guid.NewGuid(), Guid.Empty, 5);

            // Assert
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.BookId)
                  .WithErrorMessage("BookId is required.");
        }

        [Fact]
        public void Validator_ShouldHaveError_WhenQuantityIsZeroOrLess()
        {
            // Arrange
            var command = new UpdateItemQuantityCommand(Guid.NewGuid(), Guid.NewGuid(), 0);

            // Assert
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Quantity)
                  .WithErrorMessage("Quantity must be between 1 and 1000.");
        }

        [Fact]
        public void Validator_ShouldHaveError_WhenQuantityExceeds1000()
        {
            // Arrange
            var command = new UpdateItemQuantityCommand(Guid.NewGuid(), Guid.NewGuid(), 1500);

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Quantity)
                  .WithErrorMessage("Quantity must be between 1 and 1000.");
        }
    }
}

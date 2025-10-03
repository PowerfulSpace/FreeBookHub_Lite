using FluentValidation.TestHelper;
using PS.CartService.Application.CQRS.Commands.AddItem;

namespace PS.CartService.UnitTests.Application.CQRS.Commands.Addltem
{
    public class AddItemCommandValidatorTests
    {
        private readonly AddItemCommandValidator _validator = new();

        [Fact]
        public void Validator_ShouldHaveError_WhenUserIdIsEmpty()
        {
            // Arrange
            var command = new AddItemCommand(Guid.Empty, Guid.NewGuid(), 5);

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.UserId);
        }


        [Fact]
        public void Validator_ShouldHaveError_WhenBookIdIsEmpty()
        {
            // Arrange
            var command = new AddItemCommand(Guid.NewGuid(), Guid.Empty, 5);

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.BookId);
        }

        [Fact]
        public void Validator_ShouldHaveError_WhenQuantityIsZeroOrLess()
        {
            // Arrange
            var command = new AddItemCommand(Guid.NewGuid(), Guid.NewGuid(), 0);

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Quantity);
        }

        [Fact]
        public void Validator_ShouldHaveError_WhenQuantityExceeds1000()
        {
            var command = new AddItemCommand(Guid.NewGuid(), Guid.NewGuid(), 1001);

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.Quantity);
        }

        [Fact]
        public void Validator_ShouldPass_ForValidCommand()
        {
            var command = new AddItemCommand(Guid.NewGuid(), Guid.NewGuid(), 10);

            var result = _validator.TestValidate(command);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}

using FluentValidation.TestHelper;
using PS.CartService.Application.CQRS.Commands.Checkout;

namespace PS.CartService.UnitTests.Application.CQRS.Commands.Checkout
{
    public class CheckoutCommandValidatorTests
    {
        private readonly CheckoutCommandValidator _validator = new();

        [Fact]
        public void Validator_ShouldHaveError_WhenUserIdIsEmpty()
        {
            // Arrange
            var command = new CheckoutCommand(Guid.Empty, "Valid address");

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.UserId);
        }

        [Fact]
        public void Validator_ShouldHaveError_WhenShippingAddressIsEmpty()
        {
            // Arrange
            var command = new CheckoutCommand(Guid.NewGuid(), "");

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.ShippingAddress);
        }

        [Fact]
        public void Validator_ShouldHaveError_WhenShippingAddressIsTooLong()
        {
            // Arrange
            var longAddress = new string('A', 501);
            var command = new CheckoutCommand(Guid.NewGuid(), longAddress);

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.ShippingAddress);
        }

        [Fact]
        public void Validator_ShouldPass_ForValidCommand()
        {
            // Arrange
            var command = new CheckoutCommand(Guid.NewGuid(), "123 Main Street");

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}

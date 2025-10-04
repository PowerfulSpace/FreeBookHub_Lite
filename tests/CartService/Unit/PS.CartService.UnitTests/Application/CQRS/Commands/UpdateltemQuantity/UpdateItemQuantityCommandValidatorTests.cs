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
            var command = new UpdateItemQuantityCommand(Guid.Empty, Guid.NewGuid(), 5);

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.UserId)
                  .WithErrorMessage("UserId is required.");
        }
    }
}

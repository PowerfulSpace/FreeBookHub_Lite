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
            var command = new AddItemCommand(Guid.Empty, Guid.NewGuid(), 5);

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.UserId);
        }
    }
}

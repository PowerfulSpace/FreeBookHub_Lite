using FluentValidation.TestHelper;
using PS.CartService.Application.DTOs.Cart;
using PS.CartService.Application.Validators;

namespace PS.CartService.UnitTests.Application.Validators
{
    public class UpdateItemQuantityRequestValidatorTests
    {
        private readonly UpdateItemQuantityRequestValidator _validator = new();

        [Fact]
        public void Validator_ShouldHaveError_WhenBookIdIsEmpty()
        {
            var model = new UpdateItemQuantityRequest
            {
                BookId = Guid.Empty,
                Quantity = 5
            };

            var result = _validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.BookId);
        }

    }
}

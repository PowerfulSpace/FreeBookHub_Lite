using FluentValidation;
using PS.CartService.Application.DTOs.Cart;

namespace PS.CartService.Application.Validators
{
    public class UpdateItemQuantityRequestValidator : AbstractValidator<UpdateItemQuantityRequest>
    {
        public UpdateItemQuantityRequestValidator()
        {
            RuleFor(x => x.BookId)
                .NotEmpty();

            RuleFor(x => x.Quantity)
                .GreaterThanOrEqualTo(0)
                .LessThanOrEqualTo(1000);
        }
    }
}

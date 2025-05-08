using FluentValidation;
using PS.FreeBookHub_Lite.CartService.Application.DTOs;

namespace PS.FreeBookHub_Lite.CartService.Application.Validators
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

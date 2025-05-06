using FluentValidation;
using PS.FreeBookHub_Lite.CartService.Application.DTOs;

namespace PS.FreeBookHub_Lite.CartService.Application.Validators
{
    public class AddItemRequestValidator : AbstractValidator<AddItemRequest>
    {
        public AddItemRequestValidator()
        {
            RuleFor(x => x.BookId).NotEmpty();
            RuleFor(x => x.Quantity).GreaterThan(0);
        }
    }
}

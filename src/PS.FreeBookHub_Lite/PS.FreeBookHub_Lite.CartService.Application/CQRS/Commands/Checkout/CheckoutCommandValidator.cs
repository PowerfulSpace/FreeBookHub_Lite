using FluentValidation;

namespace PS.FreeBookHub_Lite.CartService.Application.CQRS.Commands.Checkout
{
    public class CheckoutCommandValidator : AbstractValidator<CheckoutCommand>
    {
        public CheckoutCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty();

            RuleFor(x => x.ShippingAddress)
                .NotEmpty()
                .MaximumLength(500)
                .WithMessage("Shipping address is required and must be up to 500 characters.");
        }
    }
}

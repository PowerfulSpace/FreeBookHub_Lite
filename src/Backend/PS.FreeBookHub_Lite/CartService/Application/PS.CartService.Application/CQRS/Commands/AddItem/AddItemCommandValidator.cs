using FluentValidation;

namespace PS.CartService.Application.CQRS.Commands.AddItem
{
    public class AddItemCommandValidator : AbstractValidator<AddItemCommand>
    {
        public AddItemCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty();

            RuleFor(x => x.BookId)
                .NotEmpty();

            RuleFor(x => x.Quantity)
                .GreaterThan(0)
                .LessThanOrEqualTo(1000)
                .WithMessage("Quantity must be between 1 and 1000.");
        }
    }
}

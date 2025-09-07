using FluentValidation;

namespace PS.CartService.Application.CQRS.Commands.UpdateItemQuantity
{
    public class UpdateItemQuantityCommandValidator : AbstractValidator<UpdateItemQuantityCommand>
    {
        public UpdateItemQuantityCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty()
                .WithMessage("UserId is required.");

            RuleFor(x => x.BookId)
                .NotEmpty()
                .WithMessage("BookId is required.");

            RuleFor(x => x.Quantity)
                .GreaterThan(0)
                .LessThanOrEqualTo(1000)
                .WithMessage("Quantity must be between 1 and 1000.");
        }
    }
}

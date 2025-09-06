using FluentValidation;

namespace PS.FreeBookHub_Lite.CatalogService.Application.CQRS.Commands.UpdateBook
{
    public class UpdateBookCommandValidator : AbstractValidator<UpdateBookCommand>
    {
        public UpdateBookCommandValidator()
        {
            RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Author).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Price)
                .GreaterThan(0)
                .PrecisionScale(18, 2, ignoreTrailingZeros: false);
            RuleFor(x => x.ISBN).NotEmpty();
            RuleFor(x => x.CoverImageUrl)
                .NotEmpty()
                .Must(uri => Uri.IsWellFormedUriString(uri, UriKind.Absolute));
        }
    }
}

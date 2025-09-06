using FluentValidation;
using PS.FreeBookHub_Lite.CatalogService.Application.DTOs;

namespace PS.FreeBookHub_Lite.CatalogService.Application.Validators
{
    public class CreateBookRequestValidator : AbstractValidator<CreateBookRequest>
    {
        public CreateBookRequestValidator()
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

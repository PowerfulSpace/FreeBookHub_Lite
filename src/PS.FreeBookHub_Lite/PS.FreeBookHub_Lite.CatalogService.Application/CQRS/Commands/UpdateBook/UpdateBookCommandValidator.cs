using FluentValidation;

namespace PS.FreeBookHub_Lite.CatalogService.Application.CQRS.Commands.UpdateBook
{
    public class UpdateBookCommandValidator : AbstractValidator<UpdateBookCommand>
    {
        public UpdateBookCommandValidator()
        {
            RuleFor(x => x.Request).NotNull();
        }
    }
}

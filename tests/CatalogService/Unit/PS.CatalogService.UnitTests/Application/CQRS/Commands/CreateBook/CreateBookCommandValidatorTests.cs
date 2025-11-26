using FluentValidation.TestHelper;
using PS.CatalogService.Application.CQRS.Commands.CreateBook;

namespace PS.CatalogService.UnitTests.Application.CQRS.Commands.CreateBook
{
    public class CreateBookCommandValidatorTests
    {
        private readonly CreateBookCommandValidator _validator = new();

        [Fact]
        public void Validator_ShouldPass_WhenDataIsValid()
        {

            var command = new CreateBookCommand
            {
                Title = "Test Book",
                Author = "John Doe",
                ISBN = "978-3-16-148410-0",
                Price = 49.99m,
                CoverImageUrl = "https://example.com/image.jpg",
                PublishedAt = DateTime.UtcNow,
                Description = "Some description"
            };


            var result = _validator.TestValidate(command);


            result.ShouldNotHaveAnyValidationErrors();
        }

    }
}

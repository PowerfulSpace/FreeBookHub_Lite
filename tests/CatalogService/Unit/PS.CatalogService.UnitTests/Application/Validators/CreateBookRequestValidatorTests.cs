using FluentValidation.TestHelper;
using PS.CatalogService.Application.DTOs;
using PS.CatalogService.Application.Validators;

namespace PS.CatalogService.UnitTests.Application.Validators
{
    public class CreateBookRequestValidatorTests
    {
        private readonly CreateBookRequestValidator _validator = new();

        [Fact]
        public void Validator_ShouldPass_WhenRequestIsValid()
        {
            var request = new CreateBookRequest
            {
                Title = "Clean Architecture",
                Author = "Robert C. Martin",
                Description = "Good book",
                ISBN = "978-0134494166",
                Price = 39.99m,
                PublishedAt = DateTime.UtcNow,
                CoverImageUrl = "https://example.com/cover.jpg"
            };

            var result = _validator.TestValidate(request);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}

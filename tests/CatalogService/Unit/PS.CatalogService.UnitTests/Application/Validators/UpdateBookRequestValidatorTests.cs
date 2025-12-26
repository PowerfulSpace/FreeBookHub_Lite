using FluentValidation.TestHelper;
using PS.CatalogService.Application.DTOs;
using PS.CatalogService.Application.Validators;

namespace PS.CatalogService.UnitTests.Application.Validators
{
    public class UpdateBookRequestValidatorTests
    {
        private readonly UpdateBookRequestValidator _validator = new();


        [Fact]
        public void Validator_ShouldPass_WhenRequestIsValid()
        {
            var request = new UpdateBookRequest
            {
                Title = "Domain-Driven Design",
                Author = "Eric Evans",
                Description = "DDD book",
                ISBN = "978-0321125217",
                Price = 49.99m,
                PublishedAt = DateTime.UtcNow,
                CoverImageUrl = "https://example.com/cover.jpg"
            };

            var result = _validator.TestValidate(request);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}

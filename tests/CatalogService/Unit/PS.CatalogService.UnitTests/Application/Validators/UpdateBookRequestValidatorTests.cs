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
            // Arrange
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

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Validator_ShouldHaveError_WhenTitleIsEmpty()
        {
            var request = new UpdateBookRequest { Title = "" };

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor(x => x.Title);
        }
    }
}

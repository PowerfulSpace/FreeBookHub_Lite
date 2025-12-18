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
            // Arrange
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

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Validator_ShouldHaveError_WhenTitleIsEmpty()
        {
            // Arrange
            var request = new CreateBookRequest { Title = "" };

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Title);
        }

        [Fact]
        public void Validator_ShouldHaveError_WhenAuthorIsEmpty()
        {
            // Arrange
            var request = new CreateBookRequest { Author = "" };

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Author);
        }

        [Fact]
        public void Validator_ShouldHaveError_WhenPriceIsZeroOrNegative()
        {
            var request = new CreateBookRequest { Price = 0 };

            var result = _validator.TestValidate(request);
            result.ShouldHaveValidationErrorFor(x => x.Price);

            request.Price = -10;
            result = _validator.TestValidate(request);
            result.ShouldHaveValidationErrorFor(x => x.Price);
        }
    }
}

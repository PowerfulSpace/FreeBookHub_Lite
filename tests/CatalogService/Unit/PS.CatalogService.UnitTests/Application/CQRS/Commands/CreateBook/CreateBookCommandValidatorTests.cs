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
            // Arrange
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

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Validator_ShouldHaveError_WhenTitleIsEmpty()
        {
            // Arrange
            var command = new CreateBookCommand { Title = "" };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Title);
        }

        [Fact]
        public void Validator_ShouldHaveError_WhenAuthorIsEmpty()
        {
            // Arrange
            var command = new CreateBookCommand { Author = "" };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Author);
        }

        [Fact]
        public void Validator_ShouldHaveError_WhenPriceIsZeroOrNegative()
        {
            // Arrange
            var command = new CreateBookCommand { Price = 0 };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Price);

            command.Price = -10;
            result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Price);
        }

        [Fact]
        public void Validator_ShouldHaveError_WhenISBNIsEmpty()
        {
            // Arrange
            var command = new CreateBookCommand { ISBN = "" };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.ISBN);
        }

        [Fact]
        public void Validator_ShouldHaveError_WhenCoverImageUrlIsInvalid()
        {
            // Arrange
            var command = new CreateBookCommand { CoverImageUrl = "invalid-url" };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.CoverImageUrl);
        }

    }
}

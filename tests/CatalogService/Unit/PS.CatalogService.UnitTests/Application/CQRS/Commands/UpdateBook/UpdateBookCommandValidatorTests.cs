using FluentValidation.TestHelper;
using PS.CatalogService.Application.CQRS.Commands.UpdateBook;

namespace PS.CatalogService.UnitTests.Application.CQRS.Commands.UpdateBook
{
    public class UpdateBookCommandValidatorTests
    {
        private readonly UpdateBookCommandValidator _validator = new();


        [Fact]
        public void Validator_ShouldPass_WhenDataIsValid()
        {
            // Arrange
            var command = new UpdateBookCommand
            {
                Id = Guid.NewGuid(),
                Title = "Updated Book",
                Author = "John Doe",
                ISBN = "978-3-16-148410-0",
                Price = 29.99m,
                CoverImageUrl = "https://example.com/book.jpg",
                Description = "Updated description",
                PublishedAt = DateTime.UtcNow
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
            var cmd = new UpdateBookCommand { Title = "" };

            // Act
            var result = _validator.TestValidate(cmd);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Title);
        }

        [Fact]
        public void Validator_ShouldHaveError_WhenAuthorIsEmpty()
        {
            // Arrange
            var cmd = new UpdateBookCommand { Author = "" };

            // Act
            var result = _validator.TestValidate(cmd);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Author);
        }

        [Fact]
        public void Validator_ShouldHaveError_WhenPriceIsZeroOrNegative()
        {
            var cmd = new UpdateBookCommand { Price = 0 };
            var result = _validator.TestValidate(cmd);
            result.ShouldHaveValidationErrorFor(x => x.Price);

            cmd.Price = -5;
            result = _validator.TestValidate(cmd);
            result.ShouldHaveValidationErrorFor(x => x.Price);
        }
    }
}

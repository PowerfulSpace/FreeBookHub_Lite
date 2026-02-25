using FluentValidation.TestHelper;
using PS.OrderService.Application.CQRS.Commands.CreateOrder;
using PS.OrderService.Application.DTOs;

namespace PS.OrderService.UnitTests.Application.CQRS.Commands.CreateOrder
{
    public class CreateOrderCommandValidatorTests
    {
        private readonly CreateOrderCommandValidator _validator = new();

        private CreateOrderCommand ValidCommand() => new()
        {
            UserId = Guid.NewGuid(),
            ShippingAddress = "Berlin, Test street 1",
            Items =
            {
                new CreateOrderItemRequest
                {
                    BookId = Guid.NewGuid(),
                    Quantity = 2,
                    UnitPrice = 10m
                }
            }
        };

        [Fact]
        public void Should_Have_Error_When_UserId_Is_Empty()
        {
            // Arrange
            var command = ValidCommand();
            command.UserId = Guid.Empty;

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.UserId);
        }

        [Fact]
        public void Should_Have_Error_When_ShippingAddress_Is_Empty()
        {
            // Arrange
            var command = ValidCommand();
            command.ShippingAddress = string.Empty;

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.ShippingAddress);
        }

        [Fact]
        public void Should_Have_Error_When_ShippingAddress_Too_Long()
        {
            var command = ValidCommand();
            command.ShippingAddress = new string('A', 201);

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.ShippingAddress);
        }

    }
}


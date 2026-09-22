using EcommerceApi.Features.Products.Commands.CreateProduct;
using FluentValidation.TestHelper;

namespace EcommerceApi.Tests.Features
{
    public class CreateProductCommandValidatorTests
    {
        private readonly CreateProductCommandValidator _validator;
        public CreateProductCommandValidatorTests()
        {
            _validator = new CreateProductCommandValidator();
        }
        [Fact]
        public void Should_Have_Error_When_Price_Is_Zero_Or_Negative()
        {
            // Arrange
            var command = new CreateProductCommand { Price = -10.00m, Name = "Laptop" };
            // Act
            var result = _validator.TestValidate(command);
            // Assert
            result.ShouldHaveValidationErrorFor(p => p.Price)
                .WithErrorMessage("Price must be greater than zero.");
        }
        [Fact]
        public void Should_Not_Have_Error_When_Command_Is_Valid()
        {
            var command = new CreateProductCommand 
            { 
                Name = "Wireless Mouse",
                Description = "High precision mouse", 
                Price = 29.99m,
                StockQuantity = 100 
            };
            var result = _validator.TestValidate(command);
            result.ShouldNotHaveAnyValidationErrors();
        }

    }
}
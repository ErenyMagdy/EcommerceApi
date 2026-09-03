using EcommerceApi.Dtos;
using EcommerceApi.Features.Products.Commands.CreateProduct;
using FluentValidation;

namespace EcommerceApi.Validators
{
    public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
    {
        public CreateProductCommandValidator()
        {
            RuleFor(p=> p.Name)
                .NotEmpty().WithMessage("Product name is required.")
                .MaximumLength(100).WithMessage("Product name cannot exceed 100 characters.");
            RuleFor(p=> p.Price)
                .GreaterThan(0).WithMessage("Price must be greater than zero.")
                .LessThan(1000000).WithMessage("Price is unreasonably high.");
            RuleFor(p=>p.StockQuantity)
                .GreaterThanOrEqualTo(0).WithMessage("Stock quantity cannot be negative.");
        }
    }
    
}

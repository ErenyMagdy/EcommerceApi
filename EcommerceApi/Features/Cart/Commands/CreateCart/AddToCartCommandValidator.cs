using FluentValidation;

namespace EcommerceApi.Features.Cart.Commands.CreateCart
{
    public class AddToCartCommandValidator:AbstractValidator<AddToCartCommand>
    {
        public AddToCartCommandValidator()
        {
            RuleFor(x => x.UserId)
              .GreaterThan(0).WithMessage("Invalid user ID.");

            RuleFor(x => x.ProductId)
                .GreaterThan(0).WithMessage("Invalid product ID.");

            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("Quantity must be at least 1.")
                .LessThanOrEqualTo(100).WithMessage("Quantity cannot exceed 100 items per request.");
        }
    }
}

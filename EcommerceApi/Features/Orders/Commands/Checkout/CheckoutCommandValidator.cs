using FluentValidation;

namespace EcommerceApi.Features.Orders.Commands.Checkout
{
    public class CheckoutCommandValidator:AbstractValidator<CheckoutCommand>
    {
        public CheckoutCommandValidator()
        {
            RuleFor(x => x.UserId)
                .GreaterThan(0).WithMessage("Invalid user ID.");
        }
    }
}

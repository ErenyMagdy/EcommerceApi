using EcommerceApi.Features.Base;

namespace EcommerceApi.Features.Orders.Commands.Checkout
{
    public class CheckoutCommand :ICommand<OrderResponse>
    {
        public int UserId { get; set; }
    }
}

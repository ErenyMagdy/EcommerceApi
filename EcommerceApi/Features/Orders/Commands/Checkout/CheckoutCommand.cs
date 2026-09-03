using EcommerceApi.Features.Base;
using System.Windows.Input;

namespace EcommerceApi.Features.Orders.Commands.Checkout
{
    public class CheckoutCommand :ICommand<OrderResponse>
    {
        public int UserId { get; set; }
    }
}

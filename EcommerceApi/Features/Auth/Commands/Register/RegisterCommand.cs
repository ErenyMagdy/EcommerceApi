using EcommerceApi.Features.Base;
using MediatR;

namespace EcommerceApi.Features.Auth.Commands.Register
{
    public class RegisterCommand:ICommand<Unit>
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}

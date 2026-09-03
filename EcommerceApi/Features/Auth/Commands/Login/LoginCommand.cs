using EcommerceApi.Features.Base;
using System.Windows.Input;

namespace EcommerceApi.Features.Auth.Commands.Login
{
    public class LoginCommand:ICommand<LoginCommandResponse>
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}

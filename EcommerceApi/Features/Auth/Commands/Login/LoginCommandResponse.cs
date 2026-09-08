namespace EcommerceApi.Features.Auth.Commands.Login
{
    public class LoginCommandResponse
    {
        public string Token { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
    }
}

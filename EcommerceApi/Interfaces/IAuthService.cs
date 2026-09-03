using EcommerceApi.Dtos;

namespace EcommerceApi.Interfaces
{
    public interface IAuthService
    {
        Task RegisterAsync(RegisterDto registerDto);
        Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
    }
}

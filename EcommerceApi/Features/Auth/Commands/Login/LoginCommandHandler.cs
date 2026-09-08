using EcommerceApi.Dtos;
using EcommerceApi.Exceptions;
using EcommerceApi.Features.Base;
using EcommerceApi.Models;
using EcommerceApi.Options;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EcommerceApi.Features.Auth.Commands.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginCommandResponse>
    {
        private readonly AppDbContext _context;

        private readonly JwtSettings _jwtSettings;
        private const int MaxFailedAttempts = 5;
        private static readonly TimeSpan LockoutDuration = TimeSpan.FromMinutes(15);
        private static readonly string dummyPasswordHash = BCrypt.Net.BCrypt.HashPassword("dummy-password");
        public LoginCommandHandler(AppDbContext context, IOptions<JwtSettings> jwtSettings)
        {
            _context = context;
            _jwtSettings = jwtSettings.Value;
        }

        public async Task<LoginCommandResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var email = request.Email.Trim().ToLowerInvariant();
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            var now = DateTimeOffset.UtcNow;
            if (user != null && user.LockoutEndUtc.HasValue && user.LockoutEndUtc.Value <= now)
            {
                user.LockoutEndUtc = null;
                user.AccessFailedCount = 0;
            }
            var passwordHash = user?.PasswordHash ?? dummyPasswordHash;
            var passwordIsValid = BCrypt.Net.BCrypt.Verify(request.Password, passwordHash);
            var accountIsLocked = user != null && user.LockoutEndUtc.HasValue && user.LockoutEndUtc.Value > now;
            if (user == null || !passwordIsValid || accountIsLocked)
            {
                if (user != null && !accountIsLocked)
                {
                    user.AccessFailedCount++;
                    if (user.AccessFailedCount >= MaxFailedAttempts)
                    {
                        user.LockoutEndUtc = now.Add(LockoutDuration);
                    }
                    await _context.SaveChangesAsync();
                }
                throw new ApiException(StatusCodes.Status401Unauthorized, "Unauthorized", "Invalid email or password.");
            }
            user.AccessFailedCount = 0;
            user.LockoutEndUtc = null;
            await _context.SaveChangesAsync();
            string token = GenerateJwtToken(user);
            return new LoginCommandResponse
            {
                Token = token,
                Role = user.Role,
                Username = user.Username
            };
        }
        private string GenerateJwtToken(User user)
        {
            var key = Encoding.UTF8.GetBytes(_jwtSettings.Secret);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role),
            };
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}

using EcommerceApi.Exceptions;
using EcommerceApi.Features.Base;
using EcommerceApi.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EcommerceApi.Features.Auth.Commands.Register
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Unit>
    {
        private readonly AppDbContext _context;
        public RegisterCommandHandler(AppDbContext context)
        {
            _context = context;
        }
        public async Task<Unit> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {

            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            {
                throw new ApiException(StatusCodes.Status409Conflict, "Email already registered", "An account already exists with this email.");
            }
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                PasswordHash = passwordHash,
                Role = "Customer"
            };

            _context.Users.Add(user);
            var cart = new Models.Cart { User = user };
            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();
            return Unit.Value;
        }
    }
}

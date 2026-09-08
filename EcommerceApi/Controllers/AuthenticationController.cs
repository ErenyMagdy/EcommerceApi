using EcommerceApi.Features.Auth.Commands.Login;
using EcommerceApi.Features.Auth.Commands.Register;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace EcommerceApi.Controllers
{
    [Route("api/Authentication")]
    [ApiController]
    [Authorize]
    public class AuthenticationController(IMediator _mediator) : ControllerBase
    {
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterCommand command)
        {
            await _mediator.Send(command);
            return Ok(new { message = "User registered successfully!" });
        }

        [HttpPost("login")]
        [EnableRateLimiting("login-limiter")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}

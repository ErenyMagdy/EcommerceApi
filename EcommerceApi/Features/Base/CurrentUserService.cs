using System.Security.Claims;

namespace EcommerceApi.Features.Base
{
    public class CurrentUserService(IHttpContextAccessor _httpContextAccessor) : ICurrentUserService
    {
        public int? UserId
        {
            get
            {
                var userIdClaim = _httpContextAccessor.HttpContext?.User?
                    .FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (int.TryParse(userIdClaim, out var parsedId))
                {
                    return parsedId;
                }
                return null;
            }
        }
        public string? UserEmail => _httpContextAccessor.HttpContext?.User?
            .FindFirst(ClaimTypes.Email)?.Value;
        public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?
            .Identity?.IsAuthenticated ?? false;
    }
}

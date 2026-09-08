namespace EcommerceApi.Features.Base
{
    public interface ICurrentUserService
    {
        int? UserId { get; }
        string? UserEmail { get; }
        bool IsAuthenticated { get; }
    }
}

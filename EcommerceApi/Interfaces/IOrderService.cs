using EcommerceApi.Dtos;

namespace EcommerceApi.Interfaces
{
    public interface IOrderService
    {
        Task<OrderDto> CheckoutAsync(int userId, CancellationToken cancellationToken = default);
        Task<List<OrderDto>> GetUserOrdersAsync(int userId, CancellationToken cancellationToken = default);
    }
}

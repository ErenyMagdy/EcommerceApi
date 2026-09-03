using EcommerceApi.Dtos;

namespace EcommerceApi.Interfaces
{
    public interface ICartService
    {
        Task<CartResponseDto> GetUserCartAsync(int userId);
        Task AddItemToCartAsync(int userId, AddToCartDto dto);
        Task RemoveItemFromCartAsync(int userId, int cartItemId);
    }
}

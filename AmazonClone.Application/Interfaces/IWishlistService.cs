using AmazonClone.Application.DTOs;
using System.Threading.Tasks;

namespace AmazonClone.Application.Interfaces
{
    public interface IWishlistService
    {
        Task<WishlistDto> GetWishlistAsync(int userId);
        Task<WishlistDto> AddItemAsync(int userId, AddWishlistItemDto dto);
        Task<bool> RemoveItemAsync(int userId, int productId);
        Task<CartDto> MoveToCartAsync(int userId, int productId);
    }
}

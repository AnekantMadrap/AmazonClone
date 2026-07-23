using AmazonClone.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AmazonClone.Application.Interfaces
{
    public interface ICartService
    {
        Task<CartDto> GetCartAsync(int userId);
        Task<CartDto> AddItemAsync(int userId, AddCartItemDto dto);
        Task<CartDto> UpdateItemQuantityAsync(int userId, int cartItemId, UpdateCartItemDto dto);
        Task<CartDto> RemoveItemAsync(int userId, int cartItemId);
        Task<bool> ClearCartAsync(int userId);
        Task<CartDto> MergeGuestCartAsync(int userId, List<GuestCartItemDto> guestItems);
    }
}

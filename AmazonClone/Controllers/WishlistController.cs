using AmazonClone.Application.DTOs;
using AmazonClone.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AmazonClone.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class WishlistController : ControllerBase
    {
        private readonly IWishlistService _wishlistService;

        public WishlistController(IWishlistService wishlistService)
        {
            _wishlistService = wishlistService;
        }

        private int GetCurrentUserId()
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdStr, out int userId) ? userId : 0;
        }

        [HttpGet]
        public async Task<IActionResult> GetWishlist()
        {
            int userId = GetCurrentUserId();
            if (userId == 0)
                return Unauthorized(new { message = "Invalid or missing user authentication." });

            var wishlist = await _wishlistService.GetWishlistAsync(userId);
            return Ok(wishlist);
        }

        [HttpPost("items")]
        public async Task<IActionResult> AddItem([FromBody] AddWishlistItemDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            int userId = GetCurrentUserId();
            if (userId == 0)
                return Unauthorized(new { message = "Invalid or missing user authentication." });

            try
            {
                var wishlist = await _wishlistService.AddItemAsync(userId, dto);
                return Ok(new { message = "Item added to wishlist successfully.", data = wishlist });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("items/{productId}")]
        public async Task<IActionResult> RemoveItem(int productId)
        {
            int userId = GetCurrentUserId();
            if (userId == 0)
                return Unauthorized(new { message = "Invalid or missing user authentication." });

            var success = await _wishlistService.RemoveItemAsync(userId, productId);
            if (!success)
                return NotFound(new { message = $"Product {productId} not found in user's wishlist." });

            return Ok(new { message = "Item removed from wishlist successfully." });
        }

        [HttpPost("items/{productId}/move-to-cart")]
        public async Task<IActionResult> MoveToCart(int productId)
        {
            int userId = GetCurrentUserId();
            if (userId == 0)
                return Unauthorized(new { message = "Invalid or missing user authentication." });

            try
            {
                var cart = await _wishlistService.MoveToCartAsync(userId, productId);
                return Ok(new { message = "Item moved to cart successfully.", data = cart });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}

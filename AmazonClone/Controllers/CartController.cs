using AmazonClone.Application.DTOs;
using AmazonClone.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AmazonClone.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        private int GetCurrentUserId()
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdStr, out int userId) ? userId : 0;
        }

        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            int userId = GetCurrentUserId();
            if (userId == 0)
                return Unauthorized(new { message = "Invalid or missing user authentication." });

            var cart = await _cartService.GetCartAsync(userId);
            return Ok(cart);
        }

        [HttpPost("items")]
        public async Task<IActionResult> AddItem([FromBody] AddCartItemDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            int userId = GetCurrentUserId();
            if (userId == 0)
                return Unauthorized(new { message = "Invalid or missing user authentication." });

            try
            {
                var cart = await _cartService.AddItemAsync(userId, dto);
                return Ok(new { message = "Item added to cart successfully.", data = cart });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("items/{id}")]
        public async Task<IActionResult> UpdateItemQuantity(int id, [FromBody] UpdateCartItemDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            int userId = GetCurrentUserId();
            if (userId == 0)
                return Unauthorized(new { message = "Invalid or missing user authentication." });

            try
            {
                var cart = await _cartService.UpdateItemQuantityAsync(userId, id, dto);
                return Ok(new { message = "Cart item updated successfully.", data = cart });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("items/{id}")]
        public async Task<IActionResult> RemoveItem(int id)
        {
            int userId = GetCurrentUserId();
            if (userId == 0)
                return Unauthorized(new { message = "Invalid or missing user authentication." });

            var cart = await _cartService.RemoveItemAsync(userId, id);
            return Ok(new { message = "Item removed from cart successfully.", data = cart });
        }

        [HttpDelete("clear")]
        public async Task<IActionResult> ClearCart()
        {
            int userId = GetCurrentUserId();
            if (userId == 0)
                return Unauthorized(new { message = "Invalid or missing user authentication." });

            await _cartService.ClearCartAsync(userId);
            return Ok(new { message = "Cart cleared successfully." });
        }

        [HttpPost("merge")]
        public async Task<IActionResult> MergeGuestCart([FromBody] List<GuestCartItemDto> guestItems)
        {
            int userId = GetCurrentUserId();
            if (userId == 0)
                return Unauthorized(new { message = "Invalid or missing user authentication." });

            var cart = await _cartService.MergeGuestCartAsync(userId, guestItems);
            return Ok(new { message = "Guest cart merged successfully.", data = cart });
        }
    }
}

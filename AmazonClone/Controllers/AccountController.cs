using AmazonClone.Application.DTOs;
using AmazonClone.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AmazonClone.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AccountController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AccountController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var profile = await _authService.GetProfileAsync(userId);
            if (profile == null)
                return NotFound(new { message = "User profile not found." });

            return Ok(profile);
        }

        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] ProfileDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var updatedProfile = await _authService.UpdateProfileAsync(userId, request);
            if (updatedProfile == null)
                return BadRequest(new { message = "Failed to update user profile." });

            return Ok(new { message = "Profile updated successfully.", data = updatedProfile });
        }
    }
}

using AmazonClone.Application.DTOs;
using AmazonClone.Application.Interfaces;
using AmazonClone.Infrastructure.Data;
using Class_Library__.NET_.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AmazonClone.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly UserManager<Users> _userManager;

        public AuthController(IAuthService authService, UserManager<Users> userManager)
        {
            _authService = authService;
            _userManager = userManager;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var ipAddress = GetClientIpAddress();
            var userAgent = Request.Headers.UserAgent.ToString();

            var result = await _authService.RegisterAsync(request, ipAddress, userAgent);

            if (!result.Succeeded)
                return BadRequest(new { errors = result.Errors });

            return Ok(result.Data);
        }

        [HttpPost("verify-email")]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.VerifyEmailAsync(request.Email, request.Token);

            if (!result.Succeeded)
                return BadRequest(new { errors = result.Errors });

            return Ok(new { message = "Email verified successfully.", data = result.Data });
        }

        [HttpGet("verify-email")]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyEmailGet([FromQuery] string email, [FromQuery] string token)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token))
                return BadRequest(new { message = "Email and token are required." });

            var result = await _authService.VerifyEmailAsync(email, token);

            if (!result.Succeeded)
                return BadRequest(new { errors = result.Errors });

            return Ok(new { message = "Email verified successfully.", data = result.Data });
        }

        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.ForgotPasswordAsync(request.Email);

            if (!result.Succeeded)
                return BadRequest(new { errors = result.Errors });

            return Ok(new { message = "If your email is registered, you will receive a password reset link shortly." });
        }

        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.ResetPasswordAsync(request.Email, request.Token, request.NewPassword);

            if (!result.Succeeded)
                return BadRequest(new { errors = result.Errors });

            return Ok(new { message = "Password has been reset successfully." });
        }

        [HttpPost("google-login")]
        [AllowAnonymous]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var ipAddress = GetClientIpAddress();
            var userAgent = Request.Headers.UserAgent.ToString();

            var result = await _authService.GoogleLoginAsync(request.IdToken, ipAddress, userAgent);

            if (!result.Succeeded)
                return BadRequest(new { errors = result.Errors });

            return Ok(result.Data);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var ipAddress = GetClientIpAddress();
            var userAgent = Request.Headers.UserAgent.ToString();

            var result = await _authService.LoginAsync(request.Email, request.Password, ipAddress, userAgent);

            if (result == null)
                return Unauthorized(new { message = "Invalid email or password" });

            return Ok(result);
        }

        [HttpPost("refresh")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var ipAddress = GetClientIpAddress();
            var userAgent = Request.Headers.UserAgent.ToString();

            var result = await _authService.RefreshTokenAsync(
                request.AccessToken,
                request.RefreshToken,
                ipAddress,
                userAgent);

            if (result == null)
                return Unauthorized(new { message = "Invalid or expired token" });

            return Ok(result);
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout([FromBody] RefreshTokenRequestDto request)
        {
            var ipAddress = GetClientIpAddress();

            var success = await _authService.LogoutAsync(
                request.AccessToken,
                request.RefreshToken,
                ipAddress);

            return success
                ? Ok(new { message = "Logged out successfully" })
                : BadRequest(new { message = "Logout failed" });
        }

        private string? GetClientIpAddress()
        {
            // Check for forwarded IP (proxy/load balancer)
            var ip = Request.Headers["X-Forwarded-For"].FirstOrDefault();

            if (string.IsNullOrEmpty(ip))
                ip = Request.HttpContext.Connection.RemoteIpAddress?.ToString();

            return ip;
        }
    }
}

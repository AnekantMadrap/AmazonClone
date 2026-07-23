using AmazonClone.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace AmazonClone.Application.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto?> LoginAsync(string email, string password, string? ipAddress, string? userAgent);
        Task<AuthResultDto> RegisterAsync(RegisterRequestDto request, string? ipAddress, string? userAgent);
        Task<AuthResponseDto?> RefreshTokenAsync(string accessToken, string refreshToken, string? ipAddress, string? userAgent);
        Task<bool> LogoutAsync(string accessToken, string refreshToken, string? ipAddress);
        Task<AuthResultDto> VerifyEmailAsync(string email, string token);
        Task<AuthResultDto> ForgotPasswordAsync(string email);
        Task<AuthResultDto> ResetPasswordAsync(string email, string token, string newPassword);

        Task<AuthResultDto> GoogleLoginAsync(string idToken, string? ipAddress, string? userAgent);
        Task<ProfileDto?> GetProfileAsync(string userId);
        Task<ProfileDto?> UpdateProfileAsync(string userId, ProfileDto request);
    }
}

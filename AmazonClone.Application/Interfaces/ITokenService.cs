using Class_Library__.NET_.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace AmazonClone.Application.Interfaces
{
    public interface ITokenService
    {
        Task<string> GenerateAccessTokenAsync(Users user, IList<string> roles);
        Task<RefreshToken> GenerateRefreshTokenAsync(Users user, string? ipAddress, string? userAgent);
        Task<Users?> ValidateAccessTokenAsync(string token);
        Task<RefreshToken?> GetValidRefreshTokenAsync(string token, int userId);
        Task RevokeRefreshTokenAsync(RefreshToken refreshToken, string? ipAddress, string? reason);
        Task<RefreshToken> RotateRefreshTokenAsync(RefreshToken oldToken, Users user, string? ipAddress, string? userAgent);
    }
}

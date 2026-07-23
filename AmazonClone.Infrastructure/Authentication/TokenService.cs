using AmazonClone.Application.Interfaces;
using AmazonClone.Infrastructure.Data;
using Class_Library__.NET_.Constants;
using Class_Library__.NET_.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AmazonClone.Infrastructure.Authentication
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Users> _userManager;

        public TokenService(
            IConfiguration config,
            ApplicationDbContext context,
            UserManager<Users> userManager)
        {
            _config = config;
            _context = context;
            _userManager = userManager;
        }

        public async Task<string> GenerateAccessTokenAsync(Users user, IList<string> roles)
        {
            var jwtKey = _config["Jwt:Key"]
                ?? throw new InvalidOperationException("JWT Key not configured");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Email, user.Email ?? string.Empty),
                new(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                new("FirstName", user.FirstName),
                new("LastName", user.LastName),
                new("UserId", user.Id.ToString())
            };

            // Add role claims
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _config["Jwt:Issuer"] ?? AuthConstants.TokenSettings.Issuer,
                Audience = _config["Jwt:Audience"] ?? AuthConstants.TokenSettings.Audience,
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(
                    int.Parse(_config["Jwt:AccessTokenExpirationMinutes"] ?? "15")),
                SigningCredentials = credentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        public async Task<RefreshToken> GenerateRefreshTokenAsync(Users user, string? ipAddress, string? userAgent)
        {
            var refreshToken = new RefreshToken
            {
                Token = Guid.NewGuid().ToString("N"), // 32-character hex string
                ExpiresUtc = DateTime.UtcNow.AddDays(
                    int.Parse(_config["Jwt:RefreshTokenExpirationDays"] ?? "7")),
                CreatedUtc = DateTime.UtcNow,
                CreatedByIp = ipAddress,
                UserAgent = userAgent,
                UserId = user.Id
            };

            user.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync();

            return refreshToken;
        }

        public async Task<Users?> ValidateAccessTokenAsync(string token)
        {
            var jwtKey = _config["Jwt:Key"];
            if (string.IsNullOrEmpty(jwtKey)) return null;

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidateIssuer = true,
                    ValidIssuer = _config["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = _config["Jwt:Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out _);

                var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim)) return null;

                var userId = int.Parse(userIdClaim);
                return await _userManager.FindByIdAsync(userId.ToString());
            }
            catch
            {
                return null;
            }
        }

        public async Task<RefreshToken?> GetValidRefreshTokenAsync(string token, int userId)
        {
            return await _context.RefreshTokens
                .FirstOrDefaultAsync(rt =>
                    rt.Token == token &&
                    rt.UserId == userId &&
                    rt.IsActive);
        }

        public async Task RevokeRefreshTokenAsync(RefreshToken refreshToken, string? ipAddress, string? reason)
        {
            refreshToken.RevokedUtc = DateTime.UtcNow;
            refreshToken.RevokedByIp = ipAddress;
            refreshToken.ReasonRevoked = reason;

            await _context.SaveChangesAsync();
        }

        public async Task<RefreshToken> RotateRefreshTokenAsync(
            RefreshToken oldToken,
            Users user,
            string? ipAddress,
            string? userAgent)
        {
            // Revoke old token
            await RevokeRefreshTokenAsync(oldToken, ipAddress, "Replaced by new token");

            // Mark as replaced (for audit trail)
            oldToken.ReplacedByToken = Guid.NewGuid().ToString("N");

            // Create new refresh token
            var newToken = await GenerateRefreshTokenAsync(user, ipAddress, userAgent);

            return newToken;
        }
    }
}
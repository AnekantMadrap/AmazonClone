using AmazonClone.Application.DTOs;
using AmazonClone.Application.Interfaces;
using Class_Library__.NET_.Entities;
using Microsoft.AspNetCore.Identity;
using AmazonClone.Infrastructure.Data;
using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;

namespace AmazonClone.Infrastructure.Authentication
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<Users> _userManager;
        private readonly ITokenService _tokenService;
        private readonly ApplicationDbContext _context;
        private readonly EmailService _emailService;
        private readonly IConfiguration _config;

        public AuthService(
            UserManager<Users> userManager,
            ITokenService tokenService,
            ApplicationDbContext context,
            EmailService emailService,
            IConfiguration config)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _context = context;
            _emailService = emailService;
            _config = config;
        }

        public async Task<AuthResponseDto?> LoginAsync(string email, string password, string? ipAddress, string? userAgent)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null || !await _userManager.CheckPasswordAsync(user, password))
                return null;

            if (!user.IsActive)
                return null;

            var roles = await _userManager.GetRolesAsync(user);
            var accessToken = await _tokenService.GenerateAccessTokenAsync(user, roles);
            var refreshToken = await _tokenService.GenerateRefreshTokenAsync(user, ipAddress, userAgent);

            return new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token,
                ExpiresAt = refreshToken.ExpiresUtc,
                UserId = user.Id.ToString(),
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email ?? string.Empty,
                Roles = roles
            };
        }

        public async Task<AuthResultDto> RegisterAsync(RegisterRequestDto request, string? ipAddress, string? userAgent)
        {
            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
            {
                return AuthResultDto.Failure("Email is already registered.");
            }

            var user = new Users
            {
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                UserName = request.Email,
                CreatedDate = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                return AuthResultDto.Failure(errors);
            }

            await _userManager.AddToRoleAsync(user, "User");

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            await _emailService.SendVerificationEmail(user.Email!, token);

            var roles = await _userManager.GetRolesAsync(user);
            var accessToken = await _tokenService.GenerateAccessTokenAsync(user, roles);
            var refreshToken = await _tokenService.GenerateRefreshTokenAsync(user, ipAddress, userAgent);

            var responseDto = new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token,
                ExpiresAt = refreshToken.ExpiresUtc,
                UserId = user.Id.ToString(),
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email ?? string.Empty,
                Roles = roles
            };

            return AuthResultDto.Success(responseDto);
        }

        public async Task<AuthResponseDto?> RefreshTokenAsync(
            string accessToken,
            string refreshToken,
            string? ipAddress,
            string? userAgent)
        {
            var user = await _tokenService.ValidateAccessTokenAsync(accessToken);
            if (user == null)
                return null;

            var storedRefreshToken = await _tokenService.GetValidRefreshTokenAsync(refreshToken, user.Id);
            if (storedRefreshToken == null)
                return null;

            var newRefreshToken = await _tokenService.RotateRefreshTokenAsync(
                storedRefreshToken, user, ipAddress, userAgent);

            var roles = await _userManager.GetRolesAsync(user);
            var newAccessToken = await _tokenService.GenerateAccessTokenAsync(user, roles);

            return new AuthResponseDto
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken.Token,
                ExpiresAt = newRefreshToken.ExpiresUtc,
                UserId = user.Id.ToString(),
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email ?? string.Empty,
                Roles = roles
            };
        }

        public async Task<bool> LogoutAsync(string accessToken, string refreshToken, string? ipAddress)
        {
            var user = await _tokenService.ValidateAccessTokenAsync(accessToken);
            if (user == null)
                return false;

            var token = await _tokenService.GetValidRefreshTokenAsync(refreshToken, user.Id);
            if (token == null)
                return false;

            await _tokenService.RevokeRefreshTokenAsync(token, ipAddress, "User logged out");
            return true;
        }

        public async Task<AuthResultDto> VerifyEmailAsync(string email, string token)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return AuthResultDto.Failure("User not found.");
            }

            if (user.EmailConfirmed)
            {
                return AuthResultDto.Failure("Email is already verified.");
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                return AuthResultDto.Failure(errors);
            }

            return AuthResultDto.Success(new AuthResponseDto
            {
                UserId = user.Id.ToString(),
                Email = user.Email ?? string.Empty,
                FirstName = user.FirstName,
                LastName = user.LastName
            });
        }

        public async Task<AuthResultDto> ForgotPasswordAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                // To prevent user enumeration attacks, return success even if email is not found
                return AuthResultDto.Success(new AuthResponseDto { Email = email });
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            await _emailService.SendPasswordResetEmail(user.Email!, token);

            return AuthResultDto.Success(new AuthResponseDto { Email = email });
        }

        public async Task<AuthResultDto> ResetPasswordAsync(string email, string token, string newPassword)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return AuthResultDto.Failure("Invalid request or token.");
            }

            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                return AuthResultDto.Failure(errors);
            }

            return AuthResultDto.Success(new AuthResponseDto
            {
                UserId = user.Id.ToString(),
                Email = user.Email ?? string.Empty,
                FirstName = user.FirstName,
                LastName = user.LastName
            });
        }

        public async Task<AuthResultDto> GoogleLoginAsync(string idToken, string? ipAddress, string? userAgent)
        {
            try
            {
                var clientId = _config["Authentication:Google:ClientId"];
                if (string.IsNullOrEmpty(clientId))
                {
                    return AuthResultDto.Failure("Google ClientId is not configured on the server.");
                }

                var settings = new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new[] { clientId }
                };

                var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);
                if (payload == null || string.IsNullOrEmpty(payload.Email))
                {
                    return AuthResultDto.Failure("Invalid Google ID token.");
                }

                var user = await _userManager.FindByEmailAsync(payload.Email);
                if (user == null)
                {
                    user = new Users
                    {
                        Email = payload.Email,
                        UserName = payload.Email,
                        FirstName = payload.GivenName ?? payload.Name ?? "Google",
                        LastName = payload.FamilyName ?? "User",
                        EmailConfirmed = payload.EmailVerified,
                        IsActive = true,
                        CreatedDate = DateTime.UtcNow
                    };

                    var createResult = await _userManager.CreateAsync(user);
                    if (!createResult.Succeeded)
                    {
                        var errors = createResult.Errors.Select(e => e.Description);
                        return AuthResultDto.Failure(errors);
                    }

                    await _userManager.AddToRoleAsync(user, "User");
                }

                if (!user.IsActive)
                {
                    return AuthResultDto.Failure("User account is deactivated.");
                }

                if (!user.EmailConfirmed && payload.EmailVerified)
                {
                    user.EmailConfirmed = true;
                    await _userManager.UpdateAsync(user);
                }

                var logins = await _userManager.GetLoginsAsync(user);
                if (!logins.Any(l => l.LoginProvider == "Google" && l.ProviderKey == payload.Subject))
                {
                    await _userManager.AddLoginAsync(user, new UserLoginInfo("Google", payload.Subject, "Google"));
                }

                var roles = await _userManager.GetRolesAsync(user);
                var accessToken = await _tokenService.GenerateAccessTokenAsync(user, roles);
                var refreshToken = await _tokenService.GenerateRefreshTokenAsync(user, ipAddress, userAgent);

                var responseDto = new AuthResponseDto
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken.Token,
                    ExpiresAt = refreshToken.ExpiresUtc,
                    UserId = user.Id.ToString(),
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email ?? string.Empty,
                    Roles = roles
                };

                return AuthResultDto.Success(responseDto);
            }
            catch (InvalidJwtException)
            {
                return AuthResultDto.Failure("Invalid or expired Google ID token.");
            }
            catch (Exception ex)
            {
                return AuthResultDto.Failure($"Google authentication failed: {ex.Message}");
            }
        }

        public async Task<ProfileDto?> GetProfileAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return null;

            var roles = await _userManager.GetRolesAsync(user);
            return new ProfileDto
            {
                UserId = user.Id.ToString(),
                Email = user.Email ?? string.Empty,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Roles = roles
            };
        }

        public async Task<ProfileDto?> UpdateProfileAsync(string userId, ProfileDto request)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return null;
            }

            user.FirstName = request.FirstName;
            user.LastName = request.LastName;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                return null;
            }

            var roles = await _userManager.GetRolesAsync(user);
            return new ProfileDto
            {
                UserId = user.Id.ToString(),
                Email = user.Email ?? string.Empty,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Roles = roles
            };
        }
    }
}
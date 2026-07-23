using AmazonClone.Infrastructure.Data;
using Class_Library__.NET_.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AmazonClone.Infrastructure.Middleware
{
    public class AuditLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<AuditLoggingMiddleware> _logger;

        public AuditLoggingMiddleware(RequestDelegate next, ILogger<AuditLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var method = context.Request.Method;
            var path = context.Request.Path.Value?.ToLower() ?? "";

            // We intercept API mutations (POST, PUT, DELETE, PATCH) or Auth endpoints
            bool isMutationOrAuth = method == "POST" || method == "PUT" || method == "DELETE" || method == "PATCH" || path.Contains("/api/auth/");

            if (!isMutationOrAuth)
            {
                await _next(context);
                return;
            }

            var ipAddress = context.Request.Headers["X-Forwarded-For"].FirstOrDefault()
                            ?? context.Connection.RemoteIpAddress?.ToString()
                            ?? "Unknown";
            var userAgent = context.Request.Headers["User-Agent"].ToString();

            // Execute the next middleware in pipeline
            await _next(context);

            // Log after execution to capture response status code and authenticated user
            var userId = context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                         ?? context.User?.FindFirst(ClaimTypes.Email)?.Value
                         ?? ipAddress;

            string action = method switch
            {
                "POST" when path.Contains("/login") => "LOGIN",
                "POST" when path.Contains("/register") => "REGISTER",
                "POST" when path.Contains("/logout") => "LOGOUT",
                "POST" => "API_CREATE",
                "PUT" => "API_UPDATE",
                "PATCH" => "API_PATCH",
                "DELETE" => "API_DELETE",
                _ => "API_REQUEST"
            };

            _logger.LogInformation("Audit Event: {Action} on {Path} by {User} (IP: {IP}, Status: {Status})",
                action, path, userId, ipAddress, context.Response.StatusCode);

            // If it's a critical auth event (login/register/logout), record explicitly into AuditLogs table
            if (action == "LOGIN" || action == "REGISTER" || action == "LOGOUT")
            {
                try
                {
                    using var scope = context.RequestServices.CreateScope();
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                    var auditLog = new AuditLog
                    {
                        TableName = "Authentication",
                        Action = action,
                        RecordId = userId,
                        NewValue = $"Status: {context.Response.StatusCode}, UserAgent: {userAgent}",
                        CreatedBy = ipAddress,
                        CreatedDate = DateTime.UtcNow
                    };

                    dbContext.AuditLogs.Add(auditLog);
                    await dbContext.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to write audit log to database for {Action}", action);
                }
            }
        }
    }
}

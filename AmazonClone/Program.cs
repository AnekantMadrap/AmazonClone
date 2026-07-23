using AmazonClone.Application.Interfaces;
using AmazonClone.Application.Services;
using AmazonClone.Infrastructure.Authentication;
using AmazonClone.Infrastructure.Data;
using AmazonClone.Infrastructure.Repositories;
using AmazonClone.Infrastructure.Services;
using Class_Library__.NET_.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using AmazonClone.Infrastructure.Middleware;
using Microsoft.AspNetCore.RateLimiting;
using Serilog;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("logs/app-.log", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 30)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
// Add Identity
builder.Services.AddIdentity<Users, ApplicationRole>(options =>
{
    options.User.RequireUniqueEmail = true;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();
// Add JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("JWT Key not configured");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// Register Services
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IProductRepository,ProductService>();
builder.Services.AddScoped<IAddressService, AddressServices>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<IBrandService, BrandService>();
builder.Services.AddScoped<IProductVariantService, ProductVariantService>();
builder.Services.AddScoped<IFileUploadService, FileUploadService>();
builder.Services.AddScoped<IInventoryService, InventoryService>();
builder.Services.AddScoped<ISqlDbConnectionFactory, SqlDbConnectionFactory>();
builder.Services.AddScoped<ICacheService, CacheService>();
builder.Services.AddScoped<IProductSearchService, ProductSearchService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IWishlistService, WishlistService>();

var redisConfig = builder.Configuration["Redis:Configuration"];
if (!string.IsNullOrWhiteSpace(redisConfig))
{
    builder.Services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = redisConfig;
        options.InstanceName = builder.Configuration["Redis:InstanceName"] ?? "AmazonClone_";
    });
}
else
{
    builder.Services.AddDistributedMemoryCache();
}

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.OnRejected = async (context, token) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        await context.HttpContext.Response.WriteAsJsonAsync(new
        {
            message = "Too many requests. Please try again later."
        }, token);
    };

    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
    {
        var ipAddress = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault()
                        ?? httpContext.Connection.RemoteIpAddress?.ToString()
                        ?? "unknown";

        return RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: ipAddress,
            factory: _ => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 100,
                QueueLimit = 0,
                Window = TimeSpan.FromMinutes(1)
            });
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    // 1. Get the DbContext
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    // 2. THIS IS THE MAGIC LINE! It automatically creates the database and ALL missing Identity tables (AspNetUsers, AspNetRoles, etc.) in SQL Server!
    await dbContext.Database.EnsureCreatedAsync();

    // 3. Now that the tables exist in SQL Server, uncomment and run your role seeder!
    await RoleSeeder.SeedRolesAsync(scope.ServiceProvider);
}


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseCors("AllowAngular");
app.UseSerilogRequestLogging();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<AuditLoggingMiddleware>();

app.MapControllers();

app.Run();

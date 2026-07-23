using Class_Library__.NET_.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace AmazonClone.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<Users, ApplicationRole, int>
    {
        private readonly IHttpContextAccessor? _httpContextAccessor;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IHttpContextAccessor? httpContextAccessor = null) : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        
        public DbSet<Products> Products { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<ProductVariant> ProductVariants { get; set; }
        public DbSet<Inventory> Inventory { get; set; }
        public DbSet<UploadedFileRecord> UploadedFiles { get; set; }

        public DbSet<Addresses> Addresses { get; set; }

        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<Categories> Categories { get; set; }

        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Wishlist> Wishlists { get; set; }
        public DbSet<WishlistItem> WishlistItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Users>(entity => entity.ToTable("Users"));
            modelBuilder.Entity<ApplicationRole>(entity => entity.ToTable("Roles"));
            modelBuilder.Entity<IdentityUserRole<int>>(entity => entity.ToTable("UserRoles"));
            modelBuilder.Entity<IdentityUserClaim<int>>(entity => entity.ToTable("UserClaims"));
            modelBuilder.Entity<IdentityUserLogin<int>>(entity => entity.ToTable("UserLogins"));
            modelBuilder.Entity<IdentityRoleClaim<int>>(entity => entity.ToTable("RoleClaims"));
            modelBuilder.Entity<IdentityUserToken<int>>(entity => entity.ToTable("UserTokens"));
            // Configure RefreshToken entity
            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.ToTable("RefreshTokens");
                entity.HasKey(e => e.Id);

                // Unique index on token
                entity.HasIndex(e => e.Token).IsUnique();

                // Relationship with Users
                entity.HasOne(e => e.User)
                      .WithMany(u => u.RefreshTokens)
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
            modelBuilder.Entity<Products>(entity =>
            {
                entity.Property(e => e.Price).HasPrecision(18, 2);
                entity.Property(e => e.DiscountPrice).HasPrecision(18, 2);
                entity.Property(e => e.Weight).HasPrecision(10, 2);
            });
            modelBuilder.Entity<ProductVariant>(entity =>
            {
                entity.Property(e => e.Price).HasPrecision(18, 2);
            });

            modelBuilder.Entity<Cart>(entity =>
            {
                entity.ToTable("Cart");
                entity.HasIndex(e => e.UserId).IsUnique();
            });

            modelBuilder.Entity<CartItem>(entity =>
            {
                entity.ToTable("CartItems");
                entity.Property(e => e.Price).HasPrecision(18, 2);
            });

            modelBuilder.Entity<Wishlist>(entity =>
            {
                entity.ToTable("Wishlists");
                entity.HasIndex(e => e.UserId).IsUnique();
            });

            modelBuilder.Entity<WishlistItem>(entity =>
            {
                entity.ToTable("WishlistItems");
            });

            // Additional Identity configurations if needed
            //modelBuilder.Entity<Users>(entity =>
            //{
            //    entity.HasIndex(e => e.Email).IsUnique();
            //});
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var auditEntries = new List<(Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry Entry, AuditLog Log)>();
            var currentUser = _httpContextAccessor?.HttpContext?.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                              ?? _httpContextAccessor?.HttpContext?.Connection?.RemoteIpAddress?.ToString()
                              ?? "System";

            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.Entity is AuditLog || entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
                    continue;

                var auditLog = new AuditLog
                {
                    TableName = entry.Entity.GetType().Name,
                    Action = entry.State.ToString().ToUpper(),
                    CreatedBy = currentUser,
                    CreatedDate = DateTime.UtcNow
                };

                var keyProperty = entry.Metadata.FindPrimaryKey()?.Properties.FirstOrDefault();
                var keyName = keyProperty?.Name;

                if (keyName != null && entry.State != EntityState.Added)
                {
                    auditLog.RecordId = entry.Property(keyName).CurrentValue?.ToString();
                }

                if (entry.State == EntityState.Modified || entry.State == EntityState.Deleted)
                {
                    var oldValues = new Dictionary<string, object?>();
                    foreach (var property in entry.Properties)
                    {
                        if (property.IsModified || entry.State == EntityState.Deleted)
                        {
                            oldValues[property.Metadata.Name] = property.OriginalValue;
                        }
                    }
                    auditLog.OldValue = JsonSerializer.Serialize(oldValues);
                }

                if (entry.State == EntityState.Modified)
                {
                    var newValues = new Dictionary<string, object?>();
                    foreach (var property in entry.Properties)
                    {
                        if (property.IsModified)
                        {
                            newValues[property.Metadata.Name] = property.CurrentValue;
                        }
                    }
                    auditLog.NewValue = JsonSerializer.Serialize(newValues);
                }

                auditEntries.Add((entry, auditLog));
            }

            var result = await base.SaveChangesAsync(cancellationToken);

            if (auditEntries.Any())
            {
                foreach (var (entry, log) in auditEntries)
                {
                    if (log.Action == "ADDED")
                    {
                        var keyProperty = entry.Metadata.FindPrimaryKey()?.Properties.FirstOrDefault();
                        if (keyProperty != null)
                        {
                            log.RecordId = entry.Property(keyProperty.Name).CurrentValue?.ToString();
                        }

                        var newValues = new Dictionary<string, object?>();
                        foreach (var property in entry.Properties)
                        {
                            newValues[property.Metadata.Name] = property.CurrentValue;
                        }
                        log.NewValue = JsonSerializer.Serialize(newValues);
                    }
                    AuditLogs.Add(log);
                }
                await base.SaveChangesAsync(cancellationToken);
            }

            return result;
        }
    }
}

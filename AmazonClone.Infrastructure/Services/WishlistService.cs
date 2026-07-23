using AmazonClone.Application.DTOs;
using AmazonClone.Application.Interfaces;
using AmazonClone.Infrastructure.Data;
using Class_Library__.NET_.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AmazonClone.Infrastructure.Services
{
    public class WishlistService : IWishlistService
    {
        private readonly ApplicationDbContext _context;
        private readonly IInventoryService _inventoryService;
        private readonly ICartService _cartService;

        public WishlistService(ApplicationDbContext context, IInventoryService inventoryService, ICartService cartService)
        {
            _context = context;
            _inventoryService = inventoryService;
            _cartService = cartService;
        }

        public async Task<WishlistDto> GetWishlistAsync(int userId)
        {
            var wishlist = await _context.Wishlists
                .Include(w => w.Items)
                    .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(w => w.UserId == userId);

            if (wishlist == null)
            {
                return new WishlistDto { UserId = userId, Items = new List<WishlistItemDto>() };
            }

            var itemDtos = new List<WishlistItemDto>();
            foreach (var item in wishlist.Items)
            {
                if (item.Product == null || item.Product.Status != "Active") continue;

                var primaryImage = await _context.UploadedFiles
                    .Where(f => f.ProductId == item.ProductId)
                    .OrderByDescending(f => f.IsPrimary)
                    .ThenBy(f => f.SortOrder)
                    .Select(f => f.FileUrl)
                    .FirstOrDefaultAsync();

                var availability = await _inventoryService.CheckAvailabilityAsync(item.ProductId, null, 1);

                itemDtos.Add(new WishlistItemDto
                {
                    WishlistItemId = item.WishlistItemId,
                    ProductId = item.ProductId,
                    ProductName = item.Product.ProductName,
                    Price = item.Product.Price,
                    DiscountPrice = item.Product.DiscountPrice,
                    PrimaryImageUrl = primaryImage,
                    AvailableStock = availability.QuantityAvailable,
                    AddedDate = item.AddedDate
                });
            }

            return new WishlistDto
            {
                WishlistId = wishlist.WishlistId,
                UserId = userId,
                Items = itemDtos
            };
        }

        public async Task<WishlistDto> AddItemAsync(int userId, AddWishlistItemDto dto)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == dto.ProductId && p.Status == "Active");
            if (product == null)
            {
                throw new InvalidOperationException("Product not found or is inactive.");
            }

            var wishlist = await _context.Wishlists.Include(w => w.Items).FirstOrDefaultAsync(w => w.UserId == userId);
            if (wishlist == null)
            {
                wishlist = new Wishlist { UserId = userId, CreatedDate = DateTime.UtcNow };
                _context.Wishlists.Add(wishlist);
                await _context.SaveChangesAsync();
            }

            if (!wishlist.Items.Any(i => i.ProductId == dto.ProductId))
            {
                _context.WishlistItems.Add(new WishlistItem
                {
                    WishlistId = wishlist.WishlistId,
                    ProductId = dto.ProductId,
                    AddedDate = DateTime.UtcNow
                });
                await _context.SaveChangesAsync();
            }

            return await GetWishlistAsync(userId);
        }

        public async Task<bool> RemoveItemAsync(int userId, int productId)
        {
            var wishlistItem = await _context.WishlistItems
                .Include(wi => wi.Wishlist)
                .FirstOrDefaultAsync(wi => wi.ProductId == productId && wi.Wishlist!.UserId == userId);

            if (wishlistItem != null)
            {
                _context.WishlistItems.Remove(wishlistItem);
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async Task<CartDto> MoveToCartAsync(int userId, int productId)
        {
            var wishlistItem = await _context.WishlistItems
                .Include(wi => wi.Wishlist)
                .FirstOrDefaultAsync(wi => wi.ProductId == productId && wi.Wishlist!.UserId == userId);

            if (wishlistItem == null)
            {
                throw new InvalidOperationException("Product not found in user's wishlist.");
            }

            // Add to cart (will perform stock validation inside _cartService.AddItemAsync)
            var updatedCart = await _cartService.AddItemAsync(userId, new AddCartItemDto
            {
                ProductId = productId,
                Quantity = 1
            });

            // If successfully added to cart without exception, remove from wishlist
            _context.WishlistItems.Remove(wishlistItem);
            await _context.SaveChangesAsync();

            return updatedCart;
        }
    }
}

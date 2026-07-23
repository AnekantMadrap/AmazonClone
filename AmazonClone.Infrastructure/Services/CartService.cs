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
    public class CartService : ICartService
    {
        private readonly ApplicationDbContext _context;
        private readonly IInventoryService _inventoryService;

        public CartService(ApplicationDbContext context, IInventoryService inventoryService)
        {
            _context = context;
            _inventoryService = inventoryService;
        }

        public async Task<CartDto> GetCartAsync(int userId)
        {
            var cart = await _context.Carts
                .Include(c => c.Items)
                    .ThenInclude(i => i.Product)
                .Include(c => c.Items)
                    .ThenInclude(i => i.Variant)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                return new CartDto { UserId = userId, Items = new List<CartItemDto>() };
            }

            var itemDtos = new List<CartItemDto>();
            foreach (var item in cart.Items)
            {
                if (item.Product == null) continue;

                var primaryImage = await _context.UploadedFiles
                    .Where(f => f.ProductId == item.ProductId)
                    .OrderByDescending(f => f.IsPrimary)
                    .ThenBy(f => f.SortOrder)
                    .Select(f => f.FileUrl)
                    .FirstOrDefaultAsync();

                var availability = await _inventoryService.CheckAvailabilityAsync(item.ProductId, item.VariantId, item.Quantity);

                string? variantInfo = null;
                if (item.Variant != null)
                {
                    var infoParts = new List<string>();
                    if (!string.IsNullOrEmpty(item.Variant.Color)) infoParts.Add($"Color: {item.Variant.Color}");
                    if (!string.IsNullOrEmpty(item.Variant.Size)) infoParts.Add($"Size: {item.Variant.Size}");
                    if (!string.IsNullOrEmpty(item.Variant.RAM)) infoParts.Add($"RAM: {item.Variant.RAM}");
                    if (!string.IsNullOrEmpty(item.Variant.Storage)) infoParts.Add($"Storage: {item.Variant.Storage}");
                    variantInfo = infoParts.Any() ? string.Join(", ", infoParts) : item.Variant.SKU;
                }

                itemDtos.Add(new CartItemDto
                {
                    CartItemId = item.CartItemId,
                    ProductId = item.ProductId,
                    ProductName = item.Product.ProductName,
                    PrimaryImageUrl = primaryImage,
                    VariantId = item.VariantId,
                    VariantInfo = variantInfo,
                    Quantity = item.Quantity,
                    UnitPrice = item.Price,
                    AvailableStock = availability.QuantityAvailable
                });
            }

            return new CartDto
            {
                CartId = cart.CartId,
                UserId = userId,
                Items = itemDtos
            };
        }

        public async Task<CartDto> AddItemAsync(int userId, AddCartItemDto dto)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == dto.ProductId && p.Status == "Active");
            if (product == null)
            {
                throw new InvalidOperationException("Product not found or is inactive.");
            }

            decimal price = product.DiscountPrice ?? product.Price;
            if (dto.VariantId.HasValue && dto.VariantId.Value > 0)
            {
                var variant = await _context.ProductVariants.FirstOrDefaultAsync(v => v.VariantId == dto.VariantId.Value && v.ProductId == dto.ProductId);
                if (variant == null)
                {
                    throw new InvalidOperationException("Variant not found for this product.");
                }
                price = variant.Price;
            }

            var cart = await _context.Carts.Include(c => c.Items).FirstOrDefaultAsync(c => c.UserId == userId);
            if (cart == null)
            {
                cart = new Cart { UserId = userId, CreatedDate = DateTime.UtcNow };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }

            var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == dto.ProductId && i.VariantId == dto.VariantId);
            int targetQuantity = existingItem != null ? existingItem.Quantity + dto.Quantity : dto.Quantity;

            var availability = await _inventoryService.CheckAvailabilityAsync(dto.ProductId, dto.VariantId, targetQuantity);
            if (!availability.IsAvailable || availability.QuantityAvailable < targetQuantity)
            {
                throw new InvalidOperationException($"Insufficient stock available. Maximum available quantity is {availability.QuantityAvailable}.");
            }

            if (existingItem != null)
            {
                existingItem.Quantity = targetQuantity;
                existingItem.Price = price;
            }
            else
            {
                _context.CartItems.Add(new CartItem
                {
                    CartId = cart.CartId,
                    ProductId = dto.ProductId,
                    VariantId = dto.VariantId,
                    Quantity = dto.Quantity,
                    Price = price
                });
            }

            await _context.SaveChangesAsync();
            return await GetCartAsync(userId);
        }

        public async Task<CartDto> UpdateItemQuantityAsync(int userId, int cartItemId, UpdateCartItemDto dto)
        {
            if (dto.Quantity <= 0)
            {
                return await RemoveItemAsync(userId, cartItemId);
            }

            var cartItem = await _context.CartItems
                .Include(ci => ci.Cart)
                .FirstOrDefaultAsync(ci => ci.CartItemId == cartItemId && ci.Cart!.UserId == userId);

            if (cartItem == null)
            {
                throw new InvalidOperationException("Cart item not found.");
            }

            var availability = await _inventoryService.CheckAvailabilityAsync(cartItem.ProductId, cartItem.VariantId, dto.Quantity);
            if (!availability.IsAvailable || availability.QuantityAvailable < dto.Quantity)
            {
                throw new InvalidOperationException($"Insufficient stock available. Maximum available quantity is {availability.QuantityAvailable}.");
            }

            cartItem.Quantity = dto.Quantity;
            await _context.SaveChangesAsync();

            return await GetCartAsync(userId);
        }

        public async Task<CartDto> RemoveItemAsync(int userId, int cartItemId)
        {
            var cartItem = await _context.CartItems
                .Include(ci => ci.Cart)
                .FirstOrDefaultAsync(ci => ci.CartItemId == cartItemId && ci.Cart!.UserId == userId);

            if (cartItem != null)
            {
                _context.CartItems.Remove(cartItem);
                await _context.SaveChangesAsync();
            }

            return await GetCartAsync(userId);
        }

        public async Task<bool> ClearCartAsync(int userId)
        {
            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart != null && cart.Items.Any())
            {
                _context.CartItems.RemoveRange(cart.Items);
                await _context.SaveChangesAsync();
            }

            return true;
        }

        public async Task<CartDto> MergeGuestCartAsync(int userId, List<GuestCartItemDto> guestItems)
        {
            if (guestItems == null || !guestItems.Any())
            {
                return await GetCartAsync(userId);
            }

            var cart = await _context.Carts.Include(c => c.Items).FirstOrDefaultAsync(c => c.UserId == userId);
            if (cart == null)
            {
                cart = new Cart { UserId = userId, CreatedDate = DateTime.UtcNow };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }

            foreach (var guestItem in guestItems)
            {
                if (guestItem.Quantity <= 0) continue;

                var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == guestItem.ProductId && p.Status == "Active");
                if (product == null) continue;

                decimal price = product.DiscountPrice ?? product.Price;
                if (guestItem.VariantId.HasValue && guestItem.VariantId.Value > 0)
                {
                    var variant = await _context.ProductVariants.FirstOrDefaultAsync(v => v.VariantId == guestItem.VariantId.Value && v.ProductId == guestItem.ProductId);
                    if (variant == null) continue;
                    price = variant.Price;
                }

                var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == guestItem.ProductId && i.VariantId == guestItem.VariantId);
                int targetQuantity = existingItem != null ? existingItem.Quantity + guestItem.Quantity : guestItem.Quantity;

                var availability = await _inventoryService.CheckAvailabilityAsync(guestItem.ProductId, guestItem.VariantId, targetQuantity);
                int finalQuantity = Math.Min(targetQuantity, availability.QuantityAvailable);
                if (finalQuantity <= 0) continue;

                if (existingItem != null)
                {
                    existingItem.Quantity = finalQuantity;
                    existingItem.Price = price;
                }
                else
                {
                    _context.CartItems.Add(new CartItem
                    {
                        CartId = cart.CartId,
                        ProductId = guestItem.ProductId,
                        VariantId = guestItem.VariantId,
                        Quantity = finalQuantity,
                        Price = price
                    });
                }
            }

            await _context.SaveChangesAsync();
            return await GetCartAsync(userId);
        }
    }
}

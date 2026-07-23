using AmazonClone.Application.DTOs;
using AmazonClone.Application.Interfaces;
using AmazonClone.Infrastructure.Data;
using Class_Library__.NET_.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AmazonClone.Infrastructure.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<InventoryService> _logger;

        public InventoryService(ApplicationDbContext context, ILogger<InventoryService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<InventoryCheckDto> CheckAvailabilityAsync(int productId, int? variantId, int requestedQuantity = 1)
        {
            if (variantId.HasValue)
            {
                var variant = await _context.ProductVariants.FirstOrDefaultAsync(v => v.VariantId == variantId.Value && v.ProductId == productId);
                if (variant == null)
                {
                    return new InventoryCheckDto
                    {
                        ProductId = productId,
                        VariantId = variantId,
                        IsAvailable = false,
                        QuantityAvailable = 0,
                        Message = "Variant not found."
                    };
                }

                bool isLowVariant = variant.StockQuantity <= 10;
                if (isLowVariant)
                {
                    _logger.LogWarning("LOW STOCK ALERT: Variant {VariantId} (Product {ProductId}) has stock {Stock} <= ReorderLevel 10", variantId, productId, variant.StockQuantity);
                }

                return new InventoryCheckDto
                {
                    ProductId = productId,
                    VariantId = variantId,
                    IsAvailable = variant.StockQuantity >= requestedQuantity,
                    QuantityAvailable = variant.StockQuantity,
                    IsLowStock = isLowVariant,
                    Message = variant.StockQuantity >= requestedQuantity ? "In Stock" : "Out of Stock or Insufficient Quantity"
                };
            }

            var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == productId);
            if (product == null)
            {
                return new InventoryCheckDto
                {
                    ProductId = productId,
                    IsAvailable = false,
                    QuantityAvailable = 0,
                    Message = "Product not found."
                };
            }
            var availableStock = await _context.Inventory
                .Where(ps => ps.ProductId == productId)
                .SumAsync(ps => (int?)ps.Quantity - ps.ReservedQuantity) ?? 0;
            bool isLowProduct = availableStock <= 10;
            if (isLowProduct)
            {
                _logger.LogWarning("LOW STOCK ALERT: Product {ProductId} has stock {Stock} <= ReorderLevel 10", productId, product.Stock);
            }

            return new InventoryCheckDto
            {
                ProductId = productId,
                IsAvailable = availableStock >= requestedQuantity,
                QuantityAvailable = availableStock,
                IsLowStock = isLowProduct,
                Message = availableStock >= requestedQuantity ? "In Stock" : "Out of Stock or Insufficient Quantity"
            };
        }

        public async Task<bool> ReserveStockAsync(int productId, int? variantId, int quantity)
        {
            if (quantity <= 0)
                return false;

            if (variantId.HasValue)
            {
                var variant = await _context.ProductVariants.FirstOrDefaultAsync(v => v.VariantId == variantId.Value && v.ProductId == productId);
                if (variant == null || variant.StockQuantity < quantity)
                    return false;

                variant.StockQuantity -= quantity;
                await _context.SaveChangesAsync();
                _logger.LogInformation("Stock reserved: {Quantity} units for Variant {VariantId} (Product {ProductId})", quantity, variantId, productId);
                return true;
            }

            var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == productId);
            if (product == null || product.Stock < quantity)
                return false;

            product.Stock -= quantity;
            await _context.SaveChangesAsync();
            _logger.LogInformation("Stock reserved: {Quantity} units for Product {ProductId}", quantity, productId);
            return true;
        }

        public async Task<bool> ValidateCartAdditionAsync(int productId, int? variantId, int quantity)
        {
            var check = await CheckAvailabilityAsync(productId, variantId, quantity);
            return check.IsAvailable;
        }
    }
}

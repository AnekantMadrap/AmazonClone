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
    public class ProductVariantService : IProductVariantService
    {
        private readonly ApplicationDbContext _context;

        public ProductVariantService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ProductVariantDto?> AddVariantAsync(int productId, ProductVariantDto variantDto)
        {
            var productExists = await _context.Products.AnyAsync(p => p.ProductId == productId);
            if (!productExists)
                return null;

            var variant = new ProductVariant
            {
                ProductId = productId,
                Color = variantDto.Color,
                Size = variantDto.Size,
                RAM = variantDto.RAM,
                Storage = variantDto.Storage,
                SKU = variantDto.SKU,
                Price = variantDto.Price,
                StockQuantity = variantDto.StockQuantity,
                IsDefault = variantDto.IsDefault
            };

            _context.ProductVariants.Add(variant);
            await _context.SaveChangesAsync();

            variantDto.VariantId = variant.VariantId;
            variantDto.ProductId = productId;
            return variantDto;
        }

        public async Task<bool> DeleteVariantAsync(int variantId)
        {
            var variant = await _context.ProductVariants.FirstOrDefaultAsync(v => v.VariantId == variantId);
            if (variant == null)
                return false;

            _context.ProductVariants.Remove(variant);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<ProductVariantDto?> GetVariantByIdAsync(int variantId)
        {
            var v = await _context.ProductVariants.FirstOrDefaultAsync(x => x.VariantId == variantId);
            if (v == null)
                return null;

            return new ProductVariantDto
            {
                VariantId = v.VariantId,
                ProductId = v.ProductId,
                Color = v.Color,
                Size = v.Size,
                RAM = v.RAM,
                Storage = v.Storage,
                SKU = v.SKU,
                Price = v.Price,
                StockQuantity = v.StockQuantity,
                IsDefault = v.IsDefault
            };
        }

        public async Task<IEnumerable<ProductVariantDto>> GetVariantsByProductIdAsync(int productId)
        {
            return await _context.ProductVariants
                .Where(v => v.ProductId == productId)
                .Select(v => new ProductVariantDto
                {
                    VariantId = v.VariantId,
                    ProductId = v.ProductId,
                    Color = v.Color,
                    Size = v.Size,
                    RAM = v.RAM,
                    Storage = v.Storage,
                    SKU = v.SKU,
                    Price = v.Price,
                    StockQuantity = v.StockQuantity,
                    IsDefault = v.IsDefault
                }).ToListAsync();
        }

        public async Task<ProductVariantDto?> UpdateVariantAsync(int variantId, ProductVariantDto variantDto)
        {
            var v = await _context.ProductVariants.FirstOrDefaultAsync(x => x.VariantId == variantId);
            if (v == null)
                return null;

            v.Color = variantDto.Color;
            v.Size = variantDto.Size;
            v.RAM = variantDto.RAM;
            v.Storage = variantDto.Storage;
            v.SKU = variantDto.SKU;
            v.Price = variantDto.Price;
            v.StockQuantity = variantDto.StockQuantity;
            v.IsDefault = variantDto.IsDefault;

            await _context.SaveChangesAsync();
            variantDto.VariantId = v.VariantId;
            return variantDto;
        }
    }
}

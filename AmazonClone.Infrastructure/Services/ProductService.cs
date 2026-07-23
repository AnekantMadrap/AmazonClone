using AmazonClone.Application.DTOs;
using AmazonClone.Application.Interfaces;
using AmazonClone.Infrastructure.Data;
using Class_Library__.NET_.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AmazonClone.Application.Services
{
    public class ProductService : IProductRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ProductDto?> AddProductAsync(ProductDto productDto)
        {
            var existingProduct = await _context.Products.FirstOrDefaultAsync(p => p.ProductName == productDto.ProductName || p.SKU == productDto.SKU);
            if (existingProduct != null)
            {
                return null;
            }

            var product = new Products
            {
                CategoryId = productDto.CategoryId,
                BrandId = productDto.BrandId,
                ProductName = productDto.ProductName,
                ShortDescription = productDto.ShortDescription,
                Description = productDto.Description,
                SKU = productDto.SKU,
                Price = productDto.Price,
                DiscountPrice = productDto.DiscountPrice,
                Weight = productDto.Weight,
                Status = productDto.Status,
                CreatedDate = DateTime.UtcNow
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            productDto.ProductId = product.ProductId;
            return productDto;
        }

        public async Task<bool> DeleteProductByIdAsync(int productId)
        {
            var existingProduct = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == productId);
            if (existingProduct != null)
            {
                _context.Products.Remove(existingProduct);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Where(p => p.Status == "Active")
                .Select(p => new ProductDto
                {
                    ProductId = p.ProductId,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category != null ? p.Category.CategoryName : null,
                    BrandId = p.BrandId,
                    BrandName = p.Brand != null ? p.Brand.BrandName : null,
                    ProductName = p.ProductName,
                    ShortDescription = p.ShortDescription,
                    Description = p.Description,
                    SKU = p.SKU,
                    Price = p.Price,
                    DiscountPrice = p.DiscountPrice,
                    Weight = p.Weight,
                    Status = p.Status,
                    CreatedDate = p.CreatedDate,
                    ModifiedDate = p.ModifiedDate,
                    PrimaryImageUrl = _context.UploadedFiles
                        .Where(u => u.ProductId == p.ProductId)
                        .OrderByDescending(u => u.IsPrimary)
                        .ThenBy(u => u.SortOrder)
                        .ThenBy(u => u.FileId)
                        .Select(u => u.FileUrl)
                        .FirstOrDefault()
                }).ToListAsync();

            return products;
        }

        public async Task<ProductDto?> GetProductByIdAsync(int productId)
        {
            var p = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .FirstOrDefaultAsync(x => x.ProductId == productId);

            if (p == null)
            {
                return null;
            }

            var primaryImageUrl = await _context.UploadedFiles
                .Where(u => u.ProductId == productId)
                .OrderByDescending(u => u.IsPrimary)
                .ThenBy(u => u.SortOrder)
                .ThenBy(u => u.FileId)
                .Select(u => u.FileUrl)
                .FirstOrDefaultAsync();

            var availableStock = await _context.Inventory
                .Where(ps => ps.ProductId == productId)
                .SumAsync(ps => (int?)ps.Quantity - ps.ReservedQuantity) ?? 0;

            return new ProductDto
            {
                ProductId = p.ProductId,
                CategoryId = p.CategoryId,
                CategoryName = p.Category != null ? p.Category.CategoryName : null,
                BrandId = p.BrandId,
                BrandName = p.Brand != null ? p.Brand.BrandName : null,
                ProductName = p.ProductName,
                ShortDescription = p.ShortDescription,
                Description = p.Description,
                SKU = p.SKU,
                Price = p.Price,
                DiscountPrice = p.DiscountPrice,
                Weight = p.Weight,
                Status = p.Status,
                CreatedDate = p.CreatedDate,
                ModifiedDate = p.ModifiedDate,
                Stock = availableStock,
                PrimaryImageUrl = primaryImageUrl
            };
        }

        public async Task<ProductDto?> UpdateProductByIdAsync(int productId, ProductDto productDto)
        {
            var existingProduct = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == productId);
            if (existingProduct == null)
            {
                return null;
            }

            existingProduct.CategoryId = productDto.CategoryId;
            existingProduct.BrandId = productDto.BrandId;
            existingProduct.ProductName = productDto.ProductName;
            existingProduct.ShortDescription = productDto.ShortDescription;
            existingProduct.Description = productDto.Description;
            existingProduct.SKU = productDto.SKU;
            existingProduct.Price = productDto.Price;
            existingProduct.DiscountPrice = productDto.DiscountPrice;
            existingProduct.Weight = productDto.Weight;
            existingProduct.Status = productDto.Status;
            existingProduct.ModifiedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            productDto.ProductId = existingProduct.ProductId;
            return productDto;
        }
    }
}

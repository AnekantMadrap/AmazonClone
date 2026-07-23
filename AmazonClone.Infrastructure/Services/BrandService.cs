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
    public class BrandService : IBrandService
    {
        private readonly ApplicationDbContext _context;

        public BrandService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<BrandDto?> AddBrandAsync(BrandDto brandDto)
        {
            var existing = await _context.Brands.FirstOrDefaultAsync(b => b.BrandName == brandDto.BrandName);
            if (existing != null)
                return null;

            var brand = new Brand
            {
                BrandName = brandDto.BrandName,
                LogoUrl = brandDto.LogoUrl,
                Description = brandDto.Description,
                Status = brandDto.Status,
                DisplayOrder = brandDto.DisplayOrder
            };

            _context.Brands.Add(brand);
            await _context.SaveChangesAsync();

            brandDto.BrandId = brand.BrandId;
            return brandDto;
        }

        public async Task<bool> DeleteBrandAsync(int brandId)
        {
            var brand = await _context.Brands.FirstOrDefaultAsync(b => b.BrandId == brandId);
            if (brand == null)
                return false;

            _context.Brands.Remove(brand);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<BrandDto>> GetAllBrandsAsync()
        {
            return await _context.Brands
                .Where(b => b.Status == "Active")
                .Select(b => new BrandDto
                {
                    BrandId = b.BrandId,
                    BrandName = b.BrandName,
                    LogoUrl = b.LogoUrl,
                    Description = b.Description,
                    Status = b.Status,
                    DisplayOrder = b.DisplayOrder
                }).ToListAsync();
        }

        public async Task<BrandDto?> GetBrandByIdAsync(int brandId)
        {
            var b = await _context.Brands.FirstOrDefaultAsync(x => x.BrandId == brandId);
            if (b == null)
                return null;

            return new BrandDto
            {
                BrandId = b.BrandId,
                BrandName = b.BrandName,
                LogoUrl = b.LogoUrl,
                Description = b.Description,
                Status = b.Status,
                DisplayOrder = b.DisplayOrder
            };
        }

        public async Task<BrandDto?> UpdateBrandAsync(int brandId, BrandDto brandDto)
        {
            var b = await _context.Brands.FirstOrDefaultAsync(x => x.BrandId == brandId);
            if (b == null)
                return null;

            b.BrandName = brandDto.BrandName;
            b.LogoUrl = brandDto.LogoUrl;
            b.Description = brandDto.Description;
            b.Status = brandDto.Status;
            b.DisplayOrder = brandDto.DisplayOrder;

            await _context.SaveChangesAsync();
            brandDto.BrandId = b.BrandId;
            return brandDto;
        }
    }
}

using AmazonClone.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AmazonClone.Application.Interfaces
{
    public interface IBrandService
    {
        Task<IEnumerable<BrandDto>> GetAllBrandsAsync();
        Task<BrandDto?> GetBrandByIdAsync(int brandId);
        Task<BrandDto?> AddBrandAsync(BrandDto brandDto);
        Task<BrandDto?> UpdateBrandAsync(int brandId, BrandDto brandDto);
        Task<bool> DeleteBrandAsync(int brandId);
    }
}

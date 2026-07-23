using AmazonClone.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AmazonClone.Application.Interfaces
{
    public interface IProductVariantService
    {
        Task<IEnumerable<ProductVariantDto>> GetVariantsByProductIdAsync(int productId);
        Task<ProductVariantDto?> GetVariantByIdAsync(int variantId);
        Task<ProductVariantDto?> AddVariantAsync(int productId, ProductVariantDto variantDto);
        Task<ProductVariantDto?> UpdateVariantAsync(int variantId, ProductVariantDto variantDto);
        Task<bool> DeleteVariantAsync(int variantId);
    }
}

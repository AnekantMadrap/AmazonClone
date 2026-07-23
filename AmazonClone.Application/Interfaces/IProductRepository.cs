using AmazonClone.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AmazonClone.Application.Interfaces
{
    public interface IProductRepository
    {
        Task<IEnumerable<ProductDto>> GetAllProductsAsync();
        Task<ProductDto?> GetProductByIdAsync(int productId);
        Task<ProductDto?> AddProductAsync(ProductDto productDto);
        Task<ProductDto?> UpdateProductByIdAsync(int productId, ProductDto productDto);
        Task<bool> DeleteProductByIdAsync(int productId);
    }
}

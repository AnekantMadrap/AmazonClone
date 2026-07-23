using AmazonClone.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace AmazonClone.Application.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoriesDto>> GetAllCategoriesAsync();
        Task<CategoriesDto?> AddCategoryAsync(CategoriesDto categoryDto);
        Task<CategoriesDto?> UpdateCategoryAsync(int categoryId, CategoriesDto categoriesDto);
        Task<bool> DeleteCategoryAsync(int categoryId);
    }
}

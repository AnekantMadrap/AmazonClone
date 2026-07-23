using AmazonClone.Application.DTOs;
using AmazonClone.Application.Interfaces;
using AmazonClone.Infrastructure.Data;
using Class_Library__.NET_.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AmazonClone.Infrastructure.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ApplicationDbContext _Context;
        private readonly ICacheService _cacheService;

        private const string CategoriesCacheKey = "categories:all";

        public CategoryService(ApplicationDbContext context, ICacheService cacheService)
        {
            _Context = context;
            _cacheService = cacheService;
        }

        public async Task<CategoriesDto?> AddCategoryAsync(CategoriesDto categoryDto)
        {
            var existingCategory = _Context.Categories.FirstOrDefault(c => c.CategoryName == categoryDto.CategoryName);
            if (existingCategory != null)
            {
                return null;
            }
            var category = new Categories
            {
                CategoryName = categoryDto.CategoryName,
                ParentCategoryId = categoryDto.ParentCategoryId,
                Image = categoryDto.Image,
                DisplayOrder = categoryDto.DisplayOrder,
                Status = categoryDto.Status
            };
            _Context.Categories.Add(category);
            await _Context.SaveChangesAsync();

            await _cacheService.RemoveAsync(CategoriesCacheKey);

            categoryDto.CategoryId = category.CategoryId;
            return categoryDto;
        }

        public async Task<bool> DeleteCategoryAsync(int categoryId)
        {
            var category = _Context.Categories.FirstOrDefault(c => c.CategoryId == categoryId);
            if (category != null)
            {
                _Context.Categories.Remove(category);
                await _Context.SaveChangesAsync();

                await _cacheService.RemoveAsync(CategoriesCacheKey);

                return true;
            }
            return false;
        }

        public async Task<IEnumerable<CategoriesDto>> GetAllCategoriesAsync()
        {
            var cachedCategories = await _cacheService.GetAsync<List<CategoriesDto>>(CategoriesCacheKey);
            if (cachedCategories != null && cachedCategories.Any())
            {
                return cachedCategories;
            }

            var categories = _Context.Categories.Where(c => c.Status == "Active").Select(c => new CategoriesDto
            {
                CategoryId = c.CategoryId,
                ParentCategoryId = c.ParentCategoryId,
                CategoryName = c.CategoryName,
                Image = c.Image,
                DisplayOrder = c.DisplayOrder,
                Status = c.Status
            }).ToList();

            if (categories.Any())
            {
                await _cacheService.SetAsync(CategoriesCacheKey, categories, TimeSpan.FromHours(24));
            }

            return categories;
        }

        public async Task<CategoriesDto?> UpdateCategoryAsync(int categoryId, CategoriesDto categoriesDto)
        {
            var category = _Context.Categories.FirstOrDefault(c => c.CategoryId == categoryId);
            if (category == null)
            {
                return null;
            }
            category.CategoryName = categoriesDto.CategoryName;
            category.ParentCategoryId = categoriesDto.ParentCategoryId;
            category.Image = categoriesDto.Image;
            category.DisplayOrder = categoriesDto.DisplayOrder;
            category.Status = categoriesDto.Status;
            await _Context.SaveChangesAsync();

            await _cacheService.RemoveAsync(CategoriesCacheKey);

            return categoriesDto;
        }
    }
}

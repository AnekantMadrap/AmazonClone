using AmazonClone.Application.DTOs;
using AmazonClone.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AmazonClone.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }
        private int GetCurrentUserId()
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdStr, out int userId) ? userId : 0;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            int userId = GetCurrentUserId();
            if (userId == 0)
                return Unauthorized(new { message = "Invalid or missing user authentication." });

            var categories = await _categoryService.GetAllCategoriesAsync();
            return Ok(categories);
        }
        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] CategoriesDto categoryDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var createdCategory = await _categoryService.AddCategoryAsync(categoryDto);
            if (createdCategory == null)
                return BadRequest(new { message = "Failed to create category." });
            return Ok(new { message = "Category created successfully.", data = createdCategory });
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] CategoriesDto categoryDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var updatedCategory = await _categoryService.UpdateCategoryAsync(id, categoryDto);
            if (updatedCategory == null)
                return NotFound(new { message = $"Category with ID {id} not found." });
            return Ok(new { message = "Category updated successfully.", data = updatedCategory });
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var deleted = await _categoryService.DeleteCategoryAsync(id);
            if (!deleted)
                return NotFound(new { message = $"Category with ID {id} not found." });
            return Ok(new { message = "Category deleted successfully." });
        }
    }
}

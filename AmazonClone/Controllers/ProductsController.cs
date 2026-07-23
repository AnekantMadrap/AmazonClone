using AmazonClone.Application.DTOs;
using AmazonClone.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AmazonClone.API.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _productService;
        private readonly IProductSearchService _searchService;

        public ProductsController(IProductRepository productService, IProductSearchService searchService)
        {
            _productService = productService;
            _searchService = searchService;
        }

        private int GetCurrentUserId()
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdStr, out int userId) ? userId : 0;
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchProducts([FromQuery] ProductSearchRequestDto request)
        {
            var result = await _searchService.SearchProductsAsync(request);
            return Ok(result);
        }

        [HttpGet("suggest")]
        public async Task<IActionResult> GetSearchSuggestions([FromQuery] string q)
        {
            var suggestions = await _searchService.GetSearchSuggestionsAsync(q);
            return Ok(suggestions);
        }

        [HttpGet("{id}/details")]
        public async Task<IActionResult> GetProductDetailsPageAsync(int id)
        {
            var details = await _searchService.GetProductDetailsPageAsync(id);
            if (details == null)
            {
                return NotFound(new { message = $"Product with ID {id} not found." });
            }
            return Ok(details);
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            var products = await _productService.GetAllProductsAsync();
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductByIdAsync(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound(new { message = $"Product with ID {id} not found." });
            }
            return Ok(product);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddProduct([FromBody] ProductDto productDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdProduct = await _productService.AddProductAsync(productDto);
            if (createdProduct == null)
                return BadRequest(new { message = "Failed to create product. Product with same name or SKU already exists." });

            return CreatedAtAction(nameof(GetProductByIdAsync), new { id = createdProduct.ProductId }, new { message = "Product created successfully.", data = createdProduct });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] ProductDto productDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updatedProduct = await _productService.UpdateProductByIdAsync(id, productDto);
            if (updatedProduct == null)
                return NotFound(new { message = $"Product with ID {id} not found." });

            return Ok(new { message = "Product updated successfully.", data = updatedProduct });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var deleted = await _productService.DeleteProductByIdAsync(id);
            if (!deleted)
                return NotFound(new { message = $"Product with ID {id} not found." });

            return Ok(new { message = "Product deleted successfully." });
        }
    }
}

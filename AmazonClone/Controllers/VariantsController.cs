using AmazonClone.Application.DTOs;
using AmazonClone.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AmazonClone.API.Controllers
{
    [ApiController]
    public class VariantsController : ControllerBase
    {
        private readonly IProductVariantService _variantService;

        public VariantsController(IProductVariantService variantService)
        {
            _variantService = variantService;
        }

        [HttpGet("api/products/{productId}/variants")]
        public async Task<IActionResult> GetVariantsByProductId(int productId)
        {
            var variants = await _variantService.GetVariantsByProductIdAsync(productId);
            return Ok(variants);
        }

        [HttpPost("api/products/{productId}/variants")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddVariant(int productId, [FromBody] ProductVariantDto variantDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _variantService.AddVariantAsync(productId, variantDto);
            if (created == null)
                return BadRequest(new { message = $"Product with ID {productId} not found or invalid variant data." });

            return Ok(new { message = "Variant added successfully.", data = created });
        }

        [HttpPut("api/variants/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateVariant(int id, [FromBody] ProductVariantDto variantDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _variantService.UpdateVariantAsync(id, variantDto);
            if (updated == null)
                return NotFound(new { message = $"Variant with ID {id} not found." });

            return Ok(new { message = "Variant updated successfully.", data = updated });
        }

        [HttpDelete("api/variants/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteVariant(int id)
        {
            var deleted = await _variantService.DeleteVariantAsync(id);
            if (!deleted)
                return NotFound(new { message = $"Variant with ID {id} not found." });

            return Ok(new { message = "Variant deleted successfully." });
        }
    }
}

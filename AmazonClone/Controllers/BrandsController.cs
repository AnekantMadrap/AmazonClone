using AmazonClone.Application.DTOs;
using AmazonClone.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AmazonClone.API.Controllers
{
    [Route("api/brands")]
    [ApiController]
    public class BrandsController : ControllerBase
    {
        private readonly IBrandService _brandService;

        public BrandsController(IBrandService brandService)
        {
            _brandService = brandService;
        }

        [HttpGet]
        public async Task<IActionResult> GetBrands()
        {
            var brands = await _brandService.GetAllBrandsAsync();
            return Ok(brands);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBrandById(int id)
        {
            var brand = await _brandService.GetBrandByIdAsync(id);
            if (brand == null)
                return NotFound(new { message = $"Brand with ID {id} not found." });

            return Ok(brand);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateBrand([FromBody] BrandDto brandDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _brandService.AddBrandAsync(brandDto);
            if (created == null)
                return BadRequest(new { message = "Brand with the same name already exists." });

            return CreatedAtAction(nameof(GetBrandById), new { id = created.BrandId }, new { message = "Brand created successfully.", data = created });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateBrand(int id, [FromBody] BrandDto brandDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _brandService.UpdateBrandAsync(id, brandDto);
            if (updated == null)
                return NotFound(new { message = $"Brand with ID {id} not found." });

            return Ok(new { message = "Brand updated successfully.", data = updated });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteBrand(int id)
        {
            var deleted = await _brandService.DeleteBrandAsync(id);
            if (!deleted)
                return NotFound(new { message = $"Brand with ID {id} not found." });

            return Ok(new { message = "Brand deleted successfully." });
        }
    }
}

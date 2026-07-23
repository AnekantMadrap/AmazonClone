using AmazonClone.Application.DTOs;
using AmazonClone.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AmazonClone.API.Controllers
{
    [Route("api/inventory")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;

        public InventoryController(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        [HttpGet("check")]
        public async Task<IActionResult> CheckAvailability([FromQuery] int productId, [FromQuery] int? variantId = null, [FromQuery] int quantity = 1)
        {
            var result = await _inventoryService.CheckAvailabilityAsync(productId, variantId, quantity);
            return Ok(result);
        }

        [HttpPost("reserve")]
        [Authorize]
        public async Task<IActionResult> ReserveStock([FromBody] InventoryReserveDto reserveDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var reserved = await _inventoryService.ReserveStockAsync(reserveDto.ProductId, reserveDto.VariantId, reserveDto.Quantity);
            if (!reserved)
                return BadRequest(new { message = "Stock reservation failed. Insufficient stock or item not found." });

            return Ok(new { message = "Stock reserved successfully." });
        }

        [HttpPost("validate-cart")]
        public async Task<IActionResult> ValidateCartAddition([FromBody] InventoryReserveDto reserveDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var isValid = await _inventoryService.ValidateCartAdditionAsync(reserveDto.ProductId, reserveDto.VariantId, reserveDto.Quantity);
            if (!isValid)
                return BadRequest(new { message = "Cannot add to cart. Out of stock or requested quantity exceeds available stock." });

            return Ok(new { message = "Cart addition valid." });
        }
    }
}

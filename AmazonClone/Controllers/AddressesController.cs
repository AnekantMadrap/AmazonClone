using AmazonClone.Application.DTOs;
using AmazonClone.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AmazonClone.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AddressesController : ControllerBase
    {
        private readonly IAddressService _addressService;

        public AddressesController(IAddressService addressService)
        {
            _addressService = addressService;
        }

        private int GetCurrentUserId()
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdStr, out int userId) ? userId : 0;
        }

        [HttpGet]
        public async Task<IActionResult> GetAddresses()
        {
            int userId = GetCurrentUserId();
            if (userId == 0)
                return Unauthorized(new { message = "Invalid or missing user authentication." });

            var addresses = await _addressService.GetAddressesAsync(userId);
            return Ok(addresses);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAddressById(int id)
        {
            int userId = GetCurrentUserId();
            if (userId == 0)
                return Unauthorized(new { message = "Invalid or missing user authentication." });

            var address = await _addressService.GetAddressByIdAsync(userId, id);
            if (address == null)
                return NotFound(new { message = $"Address with ID {id} not found." });

            return Ok(address);
        }

        [HttpPost]
        public async Task<IActionResult> AddAddress([FromBody] AddressDto addressDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            int userId = GetCurrentUserId();
            if (userId == 0)
                return Unauthorized(new { message = "Invalid or missing user authentication." });

            var addedAddress = await _addressService.AddAddressAsync(userId, addressDto);
            if (addedAddress == null)
                return BadRequest(new { message = "Failed to add address." });

            return CreatedAtAction(nameof(GetAddressById), new { id = addedAddress.AddressId }, new { message = "Address added successfully.", data = addedAddress });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAddress(int id, [FromBody] AddressDto addressDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            int userId = GetCurrentUserId();
            if (userId == 0)
                return Unauthorized(new { message = "Invalid or missing user authentication." });

            var updatedAddress = await _addressService.UpdateAddressAsync(userId, id, addressDto);
            if (updatedAddress == null)
                return NotFound(new { message = $"Address with ID {id} not found or failed to update." });

            return Ok(new { message = "Address updated successfully.", data = updatedAddress });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAddress(int id)
        {
            int userId = GetCurrentUserId();
            if (userId == 0)
                return Unauthorized(new { message = "Invalid or missing user authentication." });

            var success = await _addressService.DeleteAddressAsync(userId, id);
            if (!success)
                return NotFound(new { message = $"Address with ID {id} not found or failed to delete." });

            return Ok(new { message = "Address deleted successfully." });
        }

        [HttpPatch("{id}/default")]
        public async Task<IActionResult> SetDefaultAddress(int id)
        {
            int userId = GetCurrentUserId();
            if (userId == 0)
                return Unauthorized(new { message = "Invalid or missing user authentication." });

            var success = await _addressService.SetDefaultAddressAsync(userId, id);
            if (!success)
                return NotFound(new { message = $"Address with ID {id} not found or failed to set as default." });

            return Ok(new { message = "Default address updated successfully." });
        }
    }
}

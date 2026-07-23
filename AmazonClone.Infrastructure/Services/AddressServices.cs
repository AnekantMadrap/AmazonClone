using AmazonClone.Application.DTOs;
using AmazonClone.Application.Interfaces;
using AmazonClone.Infrastructure.Data;
using Class_Library__.NET_.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AmazonClone.Infrastructure.Services
{
    public class AddressServices : IAddressService
    {
        private readonly ApplicationDbContext _context;

        public AddressServices(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AddressDto>> GetAddressesAsync(int userId)
        {
            var addresses = await _context.Addresses
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.IsDefault)
                .ThenByDescending(a => a.AddressId)
                .ToListAsync();

            return addresses.Select(MapToDto);
        }

        public async Task<AddressDto?> GetAddressByIdAsync(int userId, int addressId)
        {
            var address = await _context.Addresses
                .FirstOrDefaultAsync(a => a.AddressId == addressId && a.UserId == userId);

            return address == null ? null : MapToDto(address);
        }

        public async Task<AddressDto?> AddAddressAsync(int userId, AddressDto addressDto)
        {
            var existingAddresses = await _context.Addresses
                .Where(a => a.UserId == userId)
                .ToListAsync();

            // If this is the user's very first address, automatically make it default
            bool isFirstAddress = !existingAddresses.Any();
            if (isFirstAddress || addressDto.IsDefault)
            {
                addressDto.IsDefault = true;

                // If setting this as default, unset IsDefault on all existing addresses
                foreach (var addr in existingAddresses.Where(a => a.IsDefault))
                {
                    addr.IsDefault = false;
                }
            }

            var entity = new Addresses
            {
                UserId = userId,
                FullName = addressDto.FullName,
                Mobile = addressDto.Mobile,
                AddressLine1 = addressDto.AddressLine1,
                AddressLine2 = addressDto.AddressLine2,
                City = addressDto.City,
                State = addressDto.State,
                Country = string.IsNullOrWhiteSpace(addressDto.Country) ? "India" : addressDto.Country,
                ZipCode = addressDto.ZipCode,
                IsDefault = addressDto.IsDefault
            };

            _context.Addresses.Add(entity);
            await _context.SaveChangesAsync();

            return MapToDto(entity);
        }

        public async Task<AddressDto?> UpdateAddressAsync(int userId, int addressId, AddressDto addressDto)
        {
            // ALWAYS check both AddressId and UserId to prevent IDOR vulnerabilities
            var entity = await _context.Addresses
                .FirstOrDefaultAsync(a => a.AddressId == addressId && a.UserId == userId);

            if (entity == null)
                return null;

            // If promoting this address to default, unset default on all other addresses
            if (addressDto.IsDefault && !entity.IsDefault)
            {
                var otherDefaults = await _context.Addresses
                    .Where(a => a.UserId == userId && a.AddressId != addressId && a.IsDefault)
                    .ToListAsync();

                foreach (var addr in otherDefaults)
                {
                    addr.IsDefault = false;
                }
            }

            entity.FullName = addressDto.FullName;
            entity.Mobile = addressDto.Mobile;
            entity.AddressLine1 = addressDto.AddressLine1;
            entity.AddressLine2 = addressDto.AddressLine2;
            entity.City = addressDto.City;
            entity.State = addressDto.State;
            entity.Country = string.IsNullOrWhiteSpace(addressDto.Country) ? "India" : addressDto.Country;
            entity.ZipCode = addressDto.ZipCode;
            entity.IsDefault = addressDto.IsDefault;

            await _context.SaveChangesAsync();

            return MapToDto(entity);
        }

        public async Task<bool> DeleteAddressAsync(int userId, int addressId)
        {
            var entity = await _context.Addresses
                .FirstOrDefaultAsync(a => a.AddressId == addressId && a.UserId == userId);

            if (entity == null)
                return false;

            bool wasDefault = entity.IsDefault;
            _context.Addresses.Remove(entity);

            // If we just deleted the default address, promote the latest remaining address to be default
            if (wasDefault)
            {
                var nextAddress = await _context.Addresses
                    .Where(a => a.UserId == userId && a.AddressId != addressId)
                    .OrderByDescending(a => a.AddressId)
                    .FirstOrDefaultAsync();

                if (nextAddress != null)
                {
                    nextAddress.IsDefault = true;
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SetDefaultAddressAsync(int userId, int addressId)
        {
            var targetAddress = await _context.Addresses
                .FirstOrDefaultAsync(a => a.AddressId == addressId && a.UserId == userId);

            if (targetAddress == null)
                return false;

            if (!targetAddress.IsDefault)
            {
                var currentDefaults = await _context.Addresses
                    .Where(a => a.UserId == userId && a.IsDefault)
                    .ToListAsync();

                foreach (var addr in currentDefaults)
                {
                    addr.IsDefault = false;
                }

                targetAddress.IsDefault = true;
                await _context.SaveChangesAsync();
            }

            return true;
        }

        private static AddressDto MapToDto(Addresses entity)
        {
            return new AddressDto
            {
                AddressId = entity.AddressId,
                FullName = entity.FullName,
                Mobile = entity.Mobile,
                AddressLine1 = entity.AddressLine1,
                AddressLine2 = entity.AddressLine2,
                City = entity.City,
                State = entity.State,
                Country = entity.Country,
                ZipCode = entity.ZipCode,
                IsDefault = entity.IsDefault
            };
        }
    }
}

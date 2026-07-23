using AmazonClone.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace AmazonClone.Application.Interfaces
{
    public interface IAddressService
    {
        Task<IEnumerable<AddressDto>> GetAddressesAsync(int userId);
        Task<AddressDto?> GetAddressByIdAsync(int userId, int addressId);
        Task<AddressDto?> AddAddressAsync(int userId, AddressDto addressDto);
        Task<AddressDto?> UpdateAddressAsync(int userId, int addressId, AddressDto addressDto);
        Task<bool> DeleteAddressAsync(int userId, int addressId);
        Task<bool> SetDefaultAddressAsync(int userId, int addressId);
    }
}

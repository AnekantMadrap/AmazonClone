using AmazonClone.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AmazonClone.Application.Interfaces
{
    public interface IInventoryService
    {
        Task<InventoryCheckDto> CheckAvailabilityAsync(int productId, int? variantId, int requestedQuantity = 1);
        Task<bool> ReserveStockAsync(int productId, int? variantId, int quantity);
        Task<bool> ValidateCartAdditionAsync(int productId, int? variantId, int quantity);
    }
}

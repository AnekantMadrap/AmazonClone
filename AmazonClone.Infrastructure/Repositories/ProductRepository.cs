using AmazonClone.Application.Interfaces;
using AmazonClone.Infrastructure.Data;
using Class_Library__.NET_.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace AmazonClone.Infrastructure.Repositories
{
    public class ProductRepository
    {
        //private readonly ApplicationDbContext _context;
        //public ProductRepository(ApplicationDbContext context)
        //{
        //    _context = context;
        //}

        //public async Task<List<Products>> GetAllProductsAsync()
        //{
        //    return await _context.Products
        //        .FromSqlRaw("EXEC sp_GetAllProducts")
        //        .ToListAsync();
        //}

        //public async Task<Products> GetProductByIdAsync(int idproductId)
        //{
        //    var param = new SqlParameter("@ProductId", idproductId);

        //    // Execute Stored Procedure with parameters
        //    var products = await _context.Products
        //        .FromSqlRaw("EXEC sp_GetProductById @ProductId", param)
        //        .ToListAsync();
        //    return products.FirstOrDefault();
        //}
    }
}

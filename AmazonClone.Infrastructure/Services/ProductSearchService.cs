using AmazonClone.Application.DTOs;
using AmazonClone.Application.Interfaces;
using AmazonClone.Infrastructure.Data;
using Dapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace AmazonClone.Infrastructure.Services
{
    public class ProductSearchService : IProductSearchService
    {
        private readonly ISqlDbConnectionFactory _connectionFactory;
        private readonly ICacheService _cacheService;
        private readonly ILogger<ProductSearchService> _logger;

        private const string BestSellersCacheKey = "products:bestsellers";

        public ProductSearchService(
            ISqlDbConnectionFactory connectionFactory,
            ICacheService cacheService,
            ILogger<ProductSearchService> logger)
        {
            _connectionFactory = connectionFactory;
            _cacheService = cacheService;
            _logger = logger;
        }

        public async Task<PagedResultDto<ProductSearchItemDto>> SearchProductsAsync(ProductSearchRequestDto request)
        {
            using var connection = _connectionFactory.CreateConnection();

            // Normalize strings & IDs so that 0 or empty/whitespace means null (dynamic filtering)
            string? searchTerm = string.IsNullOrWhiteSpace(request.SearchTerm) ? null : request.SearchTerm.Trim();
            int? categoryId = request.CategoryId <= 0 ? null : request.CategoryId;
            int? brandId = request.BrandId <= 0 ? null : request.BrandId;
            decimal? minRating = request.MinRating <= 0 ? null : request.MinRating;
            int pageNumber = request.PageNumber <= 0 ? 1 : request.PageNumber;
            int pageSize = request.PageSize <= 0 ? 20 : request.PageSize;

            // Normalize SortBy values (support frontend variations like "price_asc", "popularity", etc.)
            string sortBy = (request.SortBy?.Trim().ToLowerInvariant()) switch
            {
                "price_asc" or "priceasc" => "PriceAsc",
                "price_desc" or "pricedesc" => "PriceDesc",
                "popularity" => "Popularity",
                "rating" => "Rating",
                _ => "Newest"
            };

            var parameters = new DynamicParameters();
            parameters.Add("@SearchTerm", searchTerm);
            parameters.Add("@CategoryId", categoryId);
            parameters.Add("@BrandId", brandId);
            parameters.Add("@MinPrice", request.MinPrice);
            parameters.Add("@MaxPrice", request.MaxPrice);
            parameters.Add("@MinRating", minRating);
            parameters.Add("@SortBy", sortBy);
            parameters.Add("@PageNumber", pageNumber);
            parameters.Add("@PageSize", pageSize);

            var items = await connection.QueryAsync<ProductSearchItemDto>(
                "usp_Product_Search",
                parameters,
                commandType: CommandType.StoredProcedure);

            var itemList = items.ToList();
            var totalCount = itemList.FirstOrDefault()?.TotalCount ?? 0;

            return new PagedResultDto<ProductSearchItemDto>
            {
                Items = itemList,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<IEnumerable<ProductSuggestionDto>> GetSearchSuggestionsAsync(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return Enumerable.Empty<ProductSuggestionDto>();
            }

            using var connection = _connectionFactory.CreateConnection();
            var suggestions = await connection.QueryAsync<ProductSuggestionDto>(
                "usp_Product_Suggest",
                new { Keyword = keyword },
                commandType: CommandType.StoredProcedure);

            return suggestions;
        }

        public async Task<ProductDetailsPageDto?> GetProductDetailsPageAsync(int productId)
        {
            using var connection = _connectionFactory.CreateConnection();

            // 1. Get Product scalar details
            var sqlProduct = "SELECT * FROM [dbo].[vw_ProductDetails] WHERE ProductId = @productId AND Status = 'Active'";
            var product = await connection.QueryFirstOrDefaultAsync<ProductSearchItemDto>(sqlProduct, new { productId });
            if (product == null)
            {
                return null;
            }

            // 2. Get Product Variants
            var sqlVariants = "SELECT VariantId, ProductId, Color, Size, RAM, Storage, SKU, Price, StockQuantity, IsDefault FROM [dbo].[ProductVariants] WHERE ProductId = @productId";
            var variants = await connection.QueryAsync<ProductVariantDto>(sqlVariants, new { productId });

            // 3. Get Product Images sorted by SortOrder
            var sqlImages = "SELECT FileId, ProductId, VariantId, FileUrl, FileType, IsPrimary, SortOrder FROM [dbo].[UploadedFiles] WHERE ProductId = @productId ORDER BY SortOrder ASC, FileId ASC";
            var images = await connection.QueryAsync<FileUploadResponseDto>(sqlImages, new { productId });

            // 4. Get Similar Products in the same category (exclude current)
            var searchRequest = new ProductSearchRequestDto
            {
                CategoryId = product.CategoryId,
                PageNumber = 1,
                PageSize = 6
            };
            var similarResult = await SearchProductsAsync(searchRequest);
            var similarProducts = similarResult.Items.Where(p => p.ProductId != productId).Take(5).ToList();

            // 5. Get Frequently Bought Together recommendations (top best sellers)
            var bestSellers = await GetBestSellingProductsAsync(6);
            var frequentlyBought = bestSellers.Where(p => p.ProductId != productId).Take(4).ToList();

            return new ProductDetailsPageDto
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                SKU = product.SKU,
                Price = product.Price,
                DiscountPrice = product.DiscountPrice,
                CategoryId = product.CategoryId,
                CategoryName = product.CategoryName,
                BrandId = product.BrandId,
                BrandName = product.BrandName,
                AverageRating = product.AverageRating,
                ReviewCount = product.ReviewCount,
                AvailableStock = product.AvailableStock,
                Status = product.Status,
                CreatedDate = product.CreatedDate,
                Variants = variants.ToList(),
                Images = images.ToList(),
                SimilarProducts = similarProducts,
                FrequentlyBoughtTogether = frequentlyBought
            };
        }

        public async Task<IEnumerable<ProductSearchItemDto>> GetBestSellingProductsAsync(int count = 10)
        {
            // Check Redis cache first (6-hour expiration per requirements)
            var cachedBestSellers = await _cacheService.GetAsync<List<ProductSearchItemDto>>(BestSellersCacheKey);
            if (cachedBestSellers != null && cachedBestSellers.Any())
            {
                return cachedBestSellers.Take(count);
            }

            using var connection = _connectionFactory.CreateConnection();
            var sql = @"
                SELECT TOP (@count)
                    ProductId,
                    ProductName,
                    SKU,
                    Price,
                    DiscountPrice,
                    CategoryId,
                    CategoryName,
                    BrandId,
                    BrandName,
                    AverageRating,
                    ReviewCount,
                    AvailableStock,
                    PrimaryImageUrl,
                    Status,
                    CreatedDate
                FROM [dbo].[vw_ProductDetails]
                WHERE Status = 'Active'
                ORDER BY ReviewCount DESC, AverageRating DESC, ProductId DESC";

            var bestSellers = (await connection.QueryAsync<ProductSearchItemDto>(sql, new { count = Math.Max(count, 10) })).ToList();

            if (bestSellers.Any())
            {
                await _cacheService.SetAsync(BestSellersCacheKey, bestSellers, TimeSpan.FromHours(6));
            }

            return bestSellers.Take(count);
        }
    }
}

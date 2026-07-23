using AmazonClone.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AmazonClone.Application.Interfaces
{
    public interface IProductSearchService
    {
        Task<PagedResultDto<ProductSearchItemDto>> SearchProductsAsync(ProductSearchRequestDto request);
        Task<IEnumerable<ProductSuggestionDto>> GetSearchSuggestionsAsync(string keyword);
        Task<ProductDetailsPageDto?> GetProductDetailsPageAsync(int productId);
        Task<IEnumerable<ProductSearchItemDto>> GetBestSellingProductsAsync(int count = 10);
    }
}

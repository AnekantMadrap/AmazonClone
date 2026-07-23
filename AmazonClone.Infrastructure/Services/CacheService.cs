using AmazonClone.Application.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace AmazonClone.Infrastructure.Services
{
    public class CacheService : ICacheService
    {
        private readonly IDistributedCache _cache;
        private readonly ILogger<CacheService> _logger;

        public CacheService(IDistributedCache cache, ILogger<CacheService> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            try
            {
                var cachedData = await _cache.GetStringAsync(key);
                if (string.IsNullOrWhiteSpace(cachedData))
                {
                    return default;
                }

                return JsonSerializer.Deserialize<T>(cachedData);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to retrieve cache for key {CacheKey}. Falling back to database.", key);
                return default;
            }
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? absoluteExpiration = null)
        {
            try
            {
                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = absoluteExpiration ?? TimeSpan.FromHours(1)
                };

                var serializedData = JsonSerializer.Serialize(value);
                await _cache.SetStringAsync(key, serializedData, options);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to set cache for key {CacheKey}.", key);
            }
        }

        public async Task RemoveAsync(string key)
        {
            try
            {
                await _cache.RemoveAsync(key);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to remove cache for key {CacheKey}.", key);
            }
        }
    }
}

using System;
using System.Threading.Tasks;

namespace AmazonClone.Application.Interfaces
{
    public interface ICacheService
    {
        Task<T?> GetAsync<T>(string key);
        Task SetAsync<T>(string key, T value, TimeSpan? absoluteExpiration = null);
        Task RemoveAsync(string key);
    }
}

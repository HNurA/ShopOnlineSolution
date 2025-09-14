// CacheService.cs
using Microsoft.Extensions.Caching.Memory;
using ShopOnline.Api.Services.Contracts;
using System.Collections.Concurrent;
using System.Text.Json;

namespace ShopOnline.Api.Services
{
    public class CacheService : ICacheService
    {
        private readonly IMemoryCache memoryCache;
        private readonly ILogger<CacheService> _logger;
        private readonly ConcurrentDictionary<string, bool> cacheKeys;
        private readonly TimeSpan defaultExpiration = TimeSpan.FromMinutes(30);

        public CacheService(IMemoryCache memoryCache, ILogger<CacheService> logger)
        {
            this.memoryCache = memoryCache;
            _logger = logger;
            cacheKeys = new ConcurrentDictionary<string, bool>();
        }

        public async Task<T> GetAsync<T>(string key) where T : class
        {
            try
            {
                if (memoryCache.TryGetValue(key, out var cachedValue))
                {
                    _logger.LogInformation("Cache hit for key: {CacheKey}", key);
                    return cachedValue as T;
                }

                _logger.LogInformation("Cache miss for key: {CacheKey}", key);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting cache value for key: {CacheKey}", key);
                return null;
            }
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null) where T : class
        {
            try
            {
                var cacheExpiration = expiration ?? defaultExpiration;

                var cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = cacheExpiration,
                    SlidingExpiration = TimeSpan.FromMinutes(5),
                    Priority = CacheItemPriority.Normal
                };

                memoryCache.Set(key, value, cacheEntryOptions);
                cacheKeys.TryAdd(key, true);

                _logger.LogInformation("Cache set for key: {CacheKey}, Expiration: {Expiration}", key, cacheExpiration);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting cache value for key: {CacheKey}", key);
            }
        }

        public async Task RemoveAsync(string key)
        {
            try
            {
                memoryCache.Remove(key);
                cacheKeys.TryRemove(key, out _);

                _logger.LogInformation("Cache removed for key: {CacheKey}", key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing cache value for key: {CacheKey}", key);
            }
        }

        public async Task RemoveByPatternAsync(string pattern)
        {
            try
            {
                var keysToRemove = cacheKeys.Keys.Where(key => key.Contains(pattern)).ToList();

                foreach (var key in keysToRemove)
                {
                    await RemoveAsync(key);
                }

                _logger.LogInformation("Cache cleared for pattern: {Pattern}, Removed {Count} keys", pattern, keysToRemove.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing cache values for pattern: {Pattern}", pattern);
            }
        }
    }
}
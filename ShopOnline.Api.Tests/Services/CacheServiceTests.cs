using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using ShopOnline.Api.Services;
using ShopOnline.Caching.Services;
using ShopOnline.Models.Dtos;

namespace ShopOnline.Api.Tests.Services
{
    public class CacheServiceTests
    {
        private readonly IMemoryCache memoryCache;
        private readonly Mock<ILogger<CacheService>> mockLogger;
        private readonly CacheService cacheService;

        public CacheServiceTests()
        {
            memoryCache = new MemoryCache(new MemoryCacheOptions());
            mockLogger = new Mock<ILogger<CacheService>>();
            cacheService = new CacheService(memoryCache, mockLogger.Object);
        }

        [Fact]
        public async Task GetAsync_WhenKeyExists_ShouldReturnCachedValue()
        {
            // Arrange
            var key = "test_key";
            var testProduct = new ProductDto { Id = 1, Name = "Test Product" };

            await cacheService.SetAsync(key, testProduct);

            // Act
            var result = await cacheService.GetAsync<ProductDto>(key);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(1);
            result.Name.Should().Be("Test Product");
        }

        [Fact]
        public async Task GetAsync_WhenKeyNotExists_ShouldReturnNull()
        {
            // Arrange
            var key = "non_existent_key";

            // Act
            var result = await cacheService.GetAsync<ProductDto>(key);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task SetAsync_ShouldStoreValueInCache()
        {
            // Arrange
            var key = "test_key";
            var testProduct = new ProductDto { Id = 1, Name = "Test Product" };

            // Act
            await cacheService.SetAsync(key, testProduct);

            // Assert
            var result = await cacheService.GetAsync<ProductDto>(key);
            result.Should().NotBeNull();
            result.Name.Should().Be("Test Product");
        }

        [Fact]
        public async Task RemoveAsync_ShouldRemoveValueFromCache()
        {
            // Arrange
            var key = "test_key";
            var testProduct = new ProductDto { Id = 1, Name = "Test Product" };

            await cacheService.SetAsync(key, testProduct);

            // Act
            await cacheService.RemoveAsync(key);

            // Assert
            var result = await cacheService.GetAsync<ProductDto>(key);
            result.Should().BeNull();
        }
    }
}
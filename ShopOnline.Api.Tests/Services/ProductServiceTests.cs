// Services/ProductServiceTests.cs
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using ShopOnline.Api.Entities;
using ShopOnline.Api.Repositories.Contracts;
using ShopOnline.Api.Services;
using ShopOnline.Api.Services.Contracts;
using ShopOnline.Api.Tests.Helpers;
using ShopOnline.Caching.Services.Contracts;
using ShopOnline.Models.Dtos;

namespace ShopOnline.Api.Tests.Services
{
    public class ProductServiceTests
    {
        private readonly Mock<IProductRepository> mockProductRepository;
        private readonly Mock<ILogger<ProductService>> mockLogger;
        private readonly Mock<ICacheService> mockCacheService;
        private readonly ProductService productService;

        public ProductServiceTests()
        {
            mockProductRepository = new Mock<IProductRepository>();
            mockLogger = new Mock<ILogger<ProductService>>();
            mockCacheService = new Mock<ICacheService>();

            productService = new ProductService(
                mockProductRepository.Object,
                mockLogger.Object,
                mockCacheService.Object
            );
        }

        [Fact]
        public async Task GetProducts_WhenCacheHit_ShouldReturnCachedProducts()
        {
            // Arrange
            var expectedProducts = TestDataHelper.GetTestProducts()
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    CategoryName = p.ProductCategory.Name
                }).ToList();

            mockCacheService
                .Setup(x => x.GetAsync<IEnumerable<ProductDto>>("all_products"))
                .ReturnsAsync(expectedProducts);

            // Act
            var result = await productService.GetProducts();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.Should().BeEquivalentTo(expectedProducts);

            // Verify cache was called
            mockCacheService.Verify(x => x.GetAsync<IEnumerable<ProductDto>>("all_products"), Times.Once);

            // Verify repository was NOT called
            mockProductRepository.Verify(x => x.GetItems(), Times.Never);
        }

        [Fact]
        public async Task GetProducts_WhenCacheMiss_ShouldReturnProductsFromRepository()
        {
            // Arrange
            var testProducts = TestDataHelper.GetTestProducts();

            mockCacheService
                .Setup(x => x.GetAsync<IEnumerable<ProductDto>>("all_products"))
                .ReturnsAsync((IEnumerable<ProductDto>)null);

            mockProductRepository
                .Setup(x => x.GetItems())
                .ReturnsAsync(testProducts);

            mockCacheService
                .Setup(x => x.SetAsync(It.IsAny<string>(), It.IsAny<IEnumerable<ProductDto>>(), It.IsAny<TimeSpan?>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await productService.GetProducts();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.First().Name.Should().Be("Test Product 1");

            // Verify repository was called
            mockProductRepository.Verify(x => x.GetItems(), Times.Once);

            // Verify cache was set
            mockCacheService.Verify(x => x.SetAsync("all_products", It.IsAny<IEnumerable<ProductDto>>(), It.IsAny<TimeSpan?>()), Times.Once);
        }

        [Fact]
        public async Task GetProduct_WhenProductExists_ShouldReturnProduct()
        {
            // Arrange
            var productId = 1;
            var testProduct = TestDataHelper.GetTestProducts().First();

            mockCacheService
                .Setup(x => x.GetAsync<ProductDto>($"product_{productId}"))
                .ReturnsAsync((ProductDto)null);

            mockProductRepository
                .Setup(x => x.GetItem(productId))
                .ReturnsAsync(testProduct);

            // Act
            var result = await productService.GetProduct(productId);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(productId);
            result.Name.Should().Be("Test Product 1");
        }

        [Fact]
        public async Task GetProduct_WhenProductNotExists_ShouldReturnNull()
        {
            // Arrange
            var productId = 999;

            mockCacheService
                .Setup(x => x.GetAsync<ProductDto>($"product_{productId}"))
                .ReturnsAsync((ProductDto)null);

            mockProductRepository
                .Setup(x => x.GetItem(productId))
                .ReturnsAsync((Product)null);

            // Act
            var result = await productService.GetProduct(productId);

            // Assert
            result.Should().BeNull();
        }
    }
}
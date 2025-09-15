using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ShopOnline.Api.Controllers;
using ShopOnline.Business.Services.Contracts;
using ShopOnline.Api.Tests.Helpers;
using ShopOnline.Models.Dtos;

namespace ShopOnline.Api.Tests.Controllers
{
    public class ProductControllerTests
    {
        private readonly Mock<IProductService> mockProductService;
        private readonly ProductController productController;

        public ProductControllerTests()
        {
            mockProductService = new Mock<IProductService>();
            productController = new ProductController(mockProductService.Object);
        }

        [Fact]
        public async Task GetItems_WhenProductsExist_ShouldReturnOkWithProducts()
        {
            // Arrange
            var testProducts = TestDataHelper.GetTestProducts()
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    CategoryName = p.ProductCategory.Name
                }).ToList();

            mockProductService
                .Setup(x => x.GetProducts())
                .ReturnsAsync(testProducts);

            // Act
            var result = await productController.GetItems();

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();
            var okResult = result.Result as OkObjectResult;
            var products = okResult.Value as IEnumerable<ProductDto>;
            products.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetItems_WhenNoProducts_ShouldReturnNotFound()
        {
            // Arrange
            mockProductService
                .Setup(x => x.GetProducts())
                .ReturnsAsync(Enumerable.Empty<ProductDto>());

            // Act
            var result = await productController.GetItems();

            // Assert
            result.Result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task GetItem_WhenProductExists_ShouldReturnOkWithProduct()
        {
            // Arrange
            var productId = 1;
            var testProduct = new ProductDto { Id = productId, Name = "Test Product" };

            mockProductService
                .Setup(x => x.GetProduct(productId))
                .ReturnsAsync(testProduct);

            // Act
            var result = await productController.GetItem(productId);

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();
            var okResult = result.Result as OkObjectResult;
            var product = okResult.Value as ProductDto;
            product.Id.Should().Be(productId);
        }

        [Fact]
        public async Task GetItem_WhenProductNotExists_ShouldReturnBadRequest()
        {
            // Arrange
            var productId = 999;

            mockProductService
                .Setup(x => x.GetProduct(productId))
                .ReturnsAsync((ProductDto)null);

            // Act
            var result = await productController.GetItem(productId);

            // Assert
            result.Result.Should().BeOfType<NotFoundObjectResult>();
        }
    }
}
using FluentAssertions;
using Moq;
using ShopOnline.Api.Repositories.Contracts;
using ShopOnline.Api.Tests.Helpers;
using ShopOnline.Api.Validators;
using ShopOnline.Business.Validators;
using ShopOnline.Validation.Validators;

namespace ShopOnline.Api.Tests.Validators
{
    public class ProductValidatorTests
    {
        private readonly Mock<IProductRepository> mockProductRepository;
        private readonly ProductValidator productValidator;

        public ProductValidatorTests()
        {
            mockProductRepository = new Mock<IProductRepository>();
            productValidator = new ProductValidator(mockProductRepository.Object);
        }

        [Fact]
        public async Task ValidateProductExists_WhenProductExists_ShouldReturnValid()
        {
            // Arrange
            var productId = 1;
            var testProduct = TestDataHelper.GetTestProducts().First();

            mockProductRepository
                .Setup(x => x.GetItem(productId))
                .ReturnsAsync(testProduct);

            // Act
            var result = await productValidator.ValidateProductExists(productId);

            // Assert
            result.IsValid.Should().BeTrue();
            result.ErrorMessage.Should().BeEmpty();
        }

        [Fact]
        public async Task ValidateProductExists_WhenProductNotExists_ShouldReturnInvalid()
        {
            // Arrange
            var productId = 999;

            mockProductRepository
                .Setup(x => x.GetItem(productId))
                .ReturnsAsync((ShopOnline.Api.Entities.Product)null);

            // Act
            var result = await productValidator.ValidateProductExists(productId);

            // Assert
            result.IsValid.Should().BeFalse();
            result.ErrorMessage.Should().Contain("not found");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task ValidateProductExists_WhenInvalidId_ShouldReturnInvalid(int productId)
        {
            // Act
            var result = await productValidator.ValidateProductExists(productId);

            // Assert
            result.IsValid.Should().BeFalse();
            result.ErrorMessage.Should().Contain("Valid Product ID is required");
        }
    }
}
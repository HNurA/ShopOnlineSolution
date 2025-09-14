using FluentAssertions;
using Moq;
using ShopOnline.Api.Repositories.Contracts;
using ShopOnline.Api.Tests.Helpers;
using ShopOnline.Api.Validators;
using ShopOnline.Business.Validators;
using ShopOnline.Validation.Validators;

namespace ShopOnline.Api.Tests.Validators
{
    public class CartItemValidatorTests
    {
        private readonly Mock<IProductRepository> mockProductRepository;
        private readonly Mock<IShoppingCartRepository> mockShoppingCartRepository;
        private readonly CartItemValidator cartItemValidator;

        public CartItemValidatorTests()
        {
            mockProductRepository = new Mock<IProductRepository>();
            mockShoppingCartRepository = new Mock<IShoppingCartRepository>();
            cartItemValidator = new CartItemValidator(
                mockProductRepository.Object,
                mockShoppingCartRepository.Object
            );
        }

        [Fact]
        public async Task ValidateAddItem_WhenValidItem_ShouldReturnValid()
        {
            // Arrange
            var cartItemToAdd = TestDataHelper.GetTestCartItemToAddDto();
            var testProduct = TestDataHelper.GetTestProducts().First();

            mockProductRepository
                .Setup(x => x.GetItem(cartItemToAdd.ProductId))
                .ReturnsAsync(testProduct);

            // Act
            var result = await cartItemValidator.ValidateAddItem(cartItemToAdd);

            // Assert
            result.IsValid.Should().BeTrue();
            result.ErrorMessage.Should().BeEmpty();
        }

        [Fact]
        public async Task ValidateAddItem_WhenProductNotExists_ShouldReturnInvalid()
        {
            // Arrange
            var cartItemToAdd = TestDataHelper.GetTestCartItemToAddDto();

            mockProductRepository
                .Setup(x => x.GetItem(cartItemToAdd.ProductId))
                .ReturnsAsync((ShopOnline.Api.Entities.Product)null);

            // Act
            var result = await cartItemValidator.ValidateAddItem(cartItemToAdd);

            // Assert
            result.IsValid.Should().BeFalse();
            result.ErrorMessage.Should().Contain("not found");
        }

        [Fact]
        public async Task ValidateAddItem_WhenInsufficientStock_ShouldReturnInvalid()
        {
            // Arrange
            var cartItemToAdd = TestDataHelper.GetTestCartItemToAddDto();
            cartItemToAdd.Qty = 200; // More than available stock

            var testProduct = TestDataHelper.GetTestProducts().First();
            testProduct.Qty = 100; // Available stock

            mockProductRepository
                .Setup(x => x.GetItem(cartItemToAdd.ProductId))
                .ReturnsAsync(testProduct);

            // Act
            var result = await cartItemValidator.ValidateAddItem(cartItemToAdd);

            // Assert
            result.IsValid.Should().BeFalse();
            result.ErrorMessage.Should().Contain("Insufficient stock");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task ValidateAddItem_WhenInvalidQuantity_ShouldReturnInvalid(int quantity)
        {
            // Arrange
            var cartItemToAdd = TestDataHelper.GetTestCartItemToAddDto();
            cartItemToAdd.Qty = quantity;

            // Act
            var result = await cartItemValidator.ValidateAddItem(cartItemToAdd);

            // Assert
            result.IsValid.Should().BeFalse();
            result.ErrorMessage.Should().Contain("must be greater than 0");
        }
    }
}
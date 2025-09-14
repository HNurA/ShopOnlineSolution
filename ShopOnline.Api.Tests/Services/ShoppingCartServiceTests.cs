using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using ShopOnline.Api.Repositories.Contracts;
using ShopOnline.Api.Services;
using ShopOnline.Api.Tests.Helpers;
using ShopOnline.Api.Validators.Contracts;
using ShopOnline.Business.Services;
using ShopOnline.Business.Validators.Contracts;
using ShopOnline.Models.Dtos;
using ShopOnline.Validation.Validators.Contracts;

namespace ShopOnline.Api.Tests.Services
{
    public class ShoppingCartServiceTests
    {
        private readonly Mock<IShoppingCartRepository> mockShoppingCartRepository;
        private readonly Mock<IProductRepository> mockProductRepository;
        private readonly Mock<ICartItemValidator> mockCartItemValidator;
        private readonly Mock<ILogger<ShoppingCartService>> mockLogger;
        private readonly ShoppingCartService shoppingCartService;

        public ShoppingCartServiceTests()
        {
            mockShoppingCartRepository = new Mock<IShoppingCartRepository>();
            mockProductRepository = new Mock<IProductRepository>();
            mockCartItemValidator = new Mock<ICartItemValidator>();
            mockLogger = new Mock<ILogger<ShoppingCartService>>();

            shoppingCartService = new ShoppingCartService(
                mockShoppingCartRepository.Object,
                mockProductRepository.Object,
                mockCartItemValidator.Object,
                mockLogger.Object
            );
        }

        [Fact]
        public async Task GetItems_WhenUserHasCartItems_ShouldReturnCartItems()
        {
            // Arrange
            var userId = 1;
            var testCartItems = TestDataHelper.GetTestCartItems();
            var testProducts = TestDataHelper.GetTestProducts();

            mockShoppingCartRepository
                .Setup(x => x.GetItems(userId))
                .ReturnsAsync(testCartItems);

            mockProductRepository
                .Setup(x => x.GetItems())
                .ReturnsAsync(testProducts);

            // Act
            var result = await shoppingCartService.GetItems(userId);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task AddItem_WhenValidationPasses_ShouldAddItemToCart()
        {
            // Arrange
            var cartItemToAdd = TestDataHelper.GetTestCartItemToAddDto();
            var testProduct = TestDataHelper.GetTestProducts().First();
            var testCartItem = TestDataHelper.GetTestCartItems().First();

            mockCartItemValidator
                .Setup(x => x.ValidateAddItem(cartItemToAdd))
                .ReturnsAsync((true, string.Empty));

            mockShoppingCartRepository
                .Setup(x => x.AddItem(cartItemToAdd))
                .ReturnsAsync(testCartItem);

            mockProductRepository
                .Setup(x => x.GetItem(testCartItem.ProductId))
                .ReturnsAsync(testProduct);

            // Act
            var result = await shoppingCartService.AddItem(cartItemToAdd);

            // Assert
            result.Should().NotBeNull();
            result.ProductId.Should().Be(cartItemToAdd.ProductId);
        }

        [Fact]
        public async Task AddItem_WhenValidationFails_ShouldThrowArgumentException()
        {
            // Arrange
            var cartItemToAdd = TestDataHelper.GetTestCartItemToAddDto();
            var validationError = "Product not found";

            mockCartItemValidator
                .Setup(x => x.ValidateAddItem(cartItemToAdd))
                .ReturnsAsync((false, validationError));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => shoppingCartService.AddItem(cartItemToAdd)
            );

            exception.Message.Should().Be(validationError);
        }

        [Fact]
        public async Task DeleteItem_WhenItemExists_ShouldDeleteItem()
        {
            // Arrange
            var cartItemId = 1;
            var testCartItem = TestDataHelper.GetTestCartItems().First();
            var testProduct = TestDataHelper.GetTestProducts().First();

            mockCartItemValidator
                .Setup(x => x.ValidateDeleteItem(cartItemId))
                .ReturnsAsync((true, string.Empty));

            mockShoppingCartRepository
                .Setup(x => x.DeleteItem(cartItemId))
                .ReturnsAsync(testCartItem);

            mockProductRepository
                .Setup(x => x.GetItem(testCartItem.ProductId))
                .ReturnsAsync(testProduct);

            // Act
            var result = await shoppingCartService.DeleteItem(cartItemId);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(cartItemId);
        }
    }
}
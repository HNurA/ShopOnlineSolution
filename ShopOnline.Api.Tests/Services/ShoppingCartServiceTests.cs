using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using ShopOnline.DataAccess.Repositories.Contracts;
using ShopOnline.Business.Services;
using ShopOnline.Api.Tests.Helpers;
using ShopOnline.Models.Dtos;
using Moq;

namespace ShopOnline.Api.Tests.Services
{
    public class ShoppingCartServiceTests
    {
        private readonly Mock<IShoppingCartRepository> mockShoppingCartRepository;
        private readonly Mock<IProductRepository> mockProductRepository;
        private readonly Mock<ILogger<ShoppingCartService>> mockLogger;

        // FluentValidation mocks
        private readonly Mock<IValidator<CartItemToAddDto>> mockAddItemValidator;
        private readonly Mock<IValidator<CartItemQtyUpdateDto>> mockUpdateQtyValidator;
        private readonly Mock<IValidator<int>> mockDeleteItemValidator;

        private readonly ShoppingCartService shoppingCartService;

        public ShoppingCartServiceTests()
        {
            mockShoppingCartRepository = new Mock<IShoppingCartRepository>();
            mockProductRepository = new Mock<IProductRepository>();
            mockLogger = new Mock<ILogger<ShoppingCartService>>();

            // FluentValidation mocks
            mockAddItemValidator = new Mock<IValidator<CartItemToAddDto>>();
            mockUpdateQtyValidator = new Mock<IValidator<CartItemQtyUpdateDto>>();
            mockDeleteItemValidator = new Mock<IValidator<int>>();

            shoppingCartService = new ShoppingCartService(
                mockShoppingCartRepository.Object,
                mockProductRepository.Object,
                mockLogger.Object,
                mockAddItemValidator.Object,      // ← YENİ
                mockUpdateQtyValidator.Object,    // ← YENİ
                mockDeleteItemValidator.Object    // ← YENİ
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

            // FluentValidation mock setup
            var validationResult = new ValidationResult(); // Başarılı validation
            mockAddItemValidator
                .Setup(x => x.ValidateAsync(cartItemToAdd, default))
                .ReturnsAsync(validationResult);

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
        public async Task AddItem_WhenValidationFails_ShouldThrowValidationException()
        {
            // Arrange
            var cartItemToAdd = TestDataHelper.GetTestCartItemToAddDto();

            // FluentValidation hata mock'u
            var validationResult = new ValidationResult();
            validationResult.Errors.Add(new ValidationFailure("ProductId", "Product not found"));

            mockAddItemValidator
                .Setup(x => x.ValidateAsync(cartItemToAdd, default))
                .ReturnsAsync(validationResult);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ValidationException>(
                () => shoppingCartService.AddItem(cartItemToAdd)
            );

            exception.Errors.Should().HaveCount(1);
            exception.Errors.First().ErrorMessage.Should().Be("Product not found");
        }

        [Fact]
        public async Task DeleteItem_WhenItemExists_ShouldDeleteItem()
        {
            // Arrange
            var cartItemId = 1;
            var testCartItem = TestDataHelper.GetTestCartItems().First();
            var testProduct = TestDataHelper.GetTestProducts().First();

            // FluentValidation mock setup
            var validationResult = new ValidationResult(); // Başarılı validation
            mockDeleteItemValidator
                .Setup(x => x.ValidateAsync(cartItemId, default))
                .ReturnsAsync(validationResult);

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

        [Fact]
        public async Task UpdateQty_WhenValidationPasses_ShouldUpdateQuantity()
        {
            // Arrange
            var updateDto = new CartItemQtyUpdateDto
            {
                CartItemId = 1,
                Qty = 5
            };
            var testCartItem = TestDataHelper.GetTestCartItems().First();
            var testProduct = TestDataHelper.GetTestProducts().First();

            // FluentValidation mock setup
            var validationResult = new ValidationResult(); // Başarılı validation
            mockUpdateQtyValidator
                .Setup(x => x.ValidateAsync(updateDto, default))
                .ReturnsAsync(validationResult);

            mockShoppingCartRepository
                .Setup(x => x.UpdateQty(updateDto.CartItemId, updateDto))
                .ReturnsAsync(testCartItem);

            mockProductRepository
                .Setup(x => x.GetItem(testCartItem.ProductId))
                .ReturnsAsync(testProduct);

            // Act
            var result = await shoppingCartService.UpdateQty(updateDto);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(updateDto.CartItemId);
        }
    }
}
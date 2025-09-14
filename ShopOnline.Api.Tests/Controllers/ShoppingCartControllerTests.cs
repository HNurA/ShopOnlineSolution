using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ShopOnline.Api.Controllers;
using ShopOnline.Api.Services.Contracts;
using ShopOnline.Api.Tests.Helpers;
using ShopOnline.Models.Dtos;

namespace ShopOnline.Api.Tests.Controllers
{
    public class ShoppingCartControllerTests
    {
        private readonly Mock<IShoppingCartService> mockShoppingCartService;
        private readonly ShoppingCartController shoppingCartController;

        public ShoppingCartControllerTests()
        {
            mockShoppingCartService = new Mock<IShoppingCartService>();
            shoppingCartController = new ShoppingCartController(mockShoppingCartService.Object);
        }

        [Fact]
        public async Task GetItems_WhenUserHasItems_ShouldReturnOkWithItems()
        {
            // Arrange
            var userId = 1;
            var testCartItems = new List<CartItemDto>
            {
                new CartItemDto { Id = 1, ProductId = 1, Qty = 2 },
                new CartItemDto { Id = 2, ProductId = 2, Qty = 1 }
            };

            mockShoppingCartService
                .Setup(x => x.GetItems(userId))
                .ReturnsAsync(testCartItems);

            // Act
            var result = await shoppingCartController.GetItems(userId);

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();
            var okResult = result.Result as OkObjectResult;
            var items = okResult.Value as IEnumerable<CartItemDto>;
            items.Should().HaveCount(2);
        }

        [Fact]
        public async Task PostItem_WhenValidItem_ShouldReturnCreatedAtAction()
        {
            // Arrange
            var cartItemToAdd = TestDataHelper.GetTestCartItemToAddDto();
            var createdItem = new CartItemDto { Id = 1, ProductId = 1, Qty = 2 };

            mockShoppingCartService
                .Setup(x => x.AddItem(cartItemToAdd))
                .ReturnsAsync(createdItem);

            // Act
            var result = await shoppingCartController.PostItem(cartItemToAdd);

            // Assert
            result.Result.Should().BeOfType<CreatedAtActionResult>();
            var createdResult = result.Result as CreatedAtActionResult;
            var item = createdResult.Value as CartItemDto;
            item.Id.Should().Be(1);
        }

        [Fact]
        public async Task DeleteItem_WhenItemExists_ShouldReturnOkWithDeletedItem()
        {
            // Arrange
            var itemId = 1;
            var deletedItem = new CartItemDto { Id = itemId, ProductId = 1, Qty = 2 };

            mockShoppingCartService
                .Setup(x => x.DeleteItem(itemId))
                .ReturnsAsync(deletedItem);

            // Act
            var result = await shoppingCartController.DeleteItem(itemId);

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();
            var okResult = result.Result as OkObjectResult;
            var item = okResult.Value as CartItemDto;
            item.Id.Should().Be(itemId);
        }
    }
}
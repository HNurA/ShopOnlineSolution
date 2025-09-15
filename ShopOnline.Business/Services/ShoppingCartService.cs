using ShopOnline.DataAccess.Repositories.Contracts;
using ShopOnline.Business.Services.Contracts;
using ShopOnline.Business.Extensions;
using ShopOnline.Models.Dtos;
using Microsoft.Extensions.Logging;
using FluentValidation;

namespace ShopOnline.Business.Services
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly IShoppingCartRepository shoppingCartRepository;
        private readonly IProductRepository productRepository;
        private readonly ILogger<ShoppingCartService> logger;
        private readonly IValidator<CartItemToAddDto> addItemValidator;
        private readonly IValidator<CartItemQtyUpdateDto> updateQtyValidator;
        private readonly IValidator<int> deleteItemValidator;

        public ShoppingCartService(
            IShoppingCartRepository shoppingCartRepository,
            IProductRepository productRepository,
            ILogger<ShoppingCartService> logger,
            IValidator<CartItemToAddDto> addItemValidator,
            IValidator<CartItemQtyUpdateDto> updateQtyValidator,
            IValidator<int> deleteItemValidator)
        {
            this.shoppingCartRepository = shoppingCartRepository;
            this.productRepository = productRepository;
            this.logger = logger;
            this.addItemValidator = addItemValidator;
            this.updateQtyValidator = updateQtyValidator;
            this.deleteItemValidator = deleteItemValidator;
        }

        public async Task<List<CartItemDto>> GetItems(int userId)
        {
            logger.LogInformation("Getting cart items for user ID: {UserId}", userId);

            var cartItems = await shoppingCartRepository.GetItems(userId);

            if (cartItems == null || !cartItems.Any())
            {
                logger.LogInformation("No cart items found for user ID: {UserId}", userId);
                return new List<CartItemDto>();
            }

            var products = await productRepository.GetItems();
            var result = cartItems.ConvertToDto(products).ToList();

            logger.LogInformation("Successfully retrieved {ItemCount} cart items for user ID: {UserId}", result.Count, userId);
            return result;
        }

        public async Task<CartItemDto> AddItem(CartItemToAddDto cartItemToAddDto)
        {
            logger.LogInformation("Adding item to cart - Product ID: {ProductId}, Quantity: {Quantity}, Cart ID: {CartId}",
                cartItemToAddDto.ProductId, cartItemToAddDto.Qty, cartItemToAddDto.CartId);

            // FluentValidation
            var validationResult = await addItemValidator.ValidateAsync(cartItemToAddDto);
            if (!validationResult.IsValid)
            {
                var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                logger.LogWarning("Validation failed for adding cart item: {ValidationErrors}", errors);
                throw new ValidationException(validationResult.Errors);
            }

            var cartItem = await shoppingCartRepository.AddItem(cartItemToAddDto);

            if (cartItem == null)
            {
                logger.LogError("Failed to add item to cart - Product ID: {ProductId}", cartItemToAddDto.ProductId);
                return null;
            }

            var product = await productRepository.GetItem(cartItem.ProductId);
            var result = cartItem.ConvertToDto(product);

            logger.LogInformation("Successfully added item to cart - Cart Item ID: {CartItemId}, Product: {ProductName}",
                result.Id, product.Name);
            return result;
        }

        public async Task<CartItemDto> DeleteItem(int id)
        {
            logger.LogInformation("Deleting cart item with ID: {CartItemId}", id);

            // FluentValidation
            var validationResult = await deleteItemValidator.ValidateAsync(id);
            if (!validationResult.IsValid)
            {
                var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                logger.LogWarning("Validation failed for deleting cart item: {ValidationErrors}", errors);
                throw new ValidationException(validationResult.Errors);
            }

            var cartItem = await shoppingCartRepository.DeleteItem(id);

            if (cartItem == null)
            {
                logger.LogWarning("Cart item not found for deletion - ID: {CartItemId}", id);
                return null;
            }

            var product = await productRepository.GetItem(cartItem.ProductId);
            var result = cartItem.ConvertToDto(product);

            logger.LogInformation("Successfully deleted cart item - ID: {CartItemId}, Product: {ProductName}",
                id, product?.Name);
            return result;
        }

        public async Task<CartItemDto> UpdateQty(CartItemQtyUpdateDto cartItemQtyUpdateDto)
        {
            logger.LogInformation("Updating cart item quantity - Cart Item ID: {CartItemId}, New Quantity: {Quantity}",
                cartItemQtyUpdateDto.CartItemId, cartItemQtyUpdateDto.Qty);

            // FluentValidation
            var validationResult = await updateQtyValidator.ValidateAsync(cartItemQtyUpdateDto);
            if (!validationResult.IsValid)
            {
                var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                logger.LogWarning("Validation failed for updating cart item quantity: {ValidationErrors}", errors);
                throw new ValidationException(validationResult.Errors);
            }

            var cartItem = await shoppingCartRepository.UpdateQty(cartItemQtyUpdateDto.CartItemId, cartItemQtyUpdateDto);

            if (cartItem == null)
            {
                logger.LogWarning("Cart item not found for quantity update - ID: {CartItemId}", cartItemQtyUpdateDto.CartItemId);
                return null;
            }

            var product = await productRepository.GetItem(cartItem.ProductId);
            var result = cartItem.ConvertToDto(product);

            logger.LogInformation("Successfully updated cart item quantity - ID: {CartItemId}, Product: {ProductName}, New Quantity: {Quantity}",
                cartItemQtyUpdateDto.CartItemId, product?.Name, cartItemQtyUpdateDto.Qty);
            return result;
        }

        public async Task<CartItemDto> GetItem(int id)
        {
            logger.LogInformation("Getting cart item with ID: {CartItemId}", id);

            var cartItem = await shoppingCartRepository.GetItem(id);

            if (cartItem == null)
            {
                logger.LogWarning("Cart item not found with ID: {CartItemId}", id);
                return null;
            }

            var product = await productRepository.GetItem(cartItem.ProductId);

            if (product == null)
            {
                logger.LogWarning("Product not found for cart item ID: {CartItemId}, Product ID: {ProductId}",
                    id, cartItem.ProductId);
                return null;
            }

            logger.LogInformation("Successfully retrieved cart item ID: {CartItemId}", id);
            return cartItem.ConvertToDto(product);
        }
    }
}
using ShopOnline.Api.Repositories.Contracts;
using ShopOnline.Api.Services.Contracts;
using ShopOnline.Api.Extensions;
using ShopOnline.Models.Dtos;
using ShopOnline.Validation.Validators.Contracts;
using ShopOnline.Business.Validators.Contracts;

namespace ShopOnline.Business.Services
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly IShoppingCartRepository shoppingCartRepository;
        private readonly IProductRepository productRepository;
        private readonly ICartItemValidator cartItemValidator;
        private readonly ILogger<ShoppingCartService> logger;

        public ShoppingCartService(IShoppingCartRepository shoppingCartRepository,
                                 IProductRepository productRepository,
                                 ICartItemValidator cartItemValidator,
                                 ILogger<ShoppingCartService> logger) 
        {
            this.shoppingCartRepository = shoppingCartRepository;
            this.productRepository = productRepository;
            this.cartItemValidator = cartItemValidator;
            this.logger = logger; 
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
            logger.LogInformation("Adding item to cart - Product ID: {ProductId}, Quantity: {Quantity}, Cart ID: {CartId}", cartItemToAddDto.ProductId, cartItemToAddDto.Qty, cartItemToAddDto.CartId); // ← YENİ EKLENEN

            /*
            // Business Logic: Ürün var mı kontrol et
            var product = await productRepository.GetItem(cartItemToAddDto.ProductId);
            if (product == null)
            {
                throw new Exception("Product not found");
            */

            // Validation
            var validationResult = await cartItemValidator.ValidateAddItem(cartItemToAddDto);
            if (!validationResult.IsValid)
            {
                logger.LogWarning("Validation failed for adding cart item: {ValidationError}", validationResult.ErrorMessage); 
                throw new ArgumentException(validationResult.ErrorMessage);
            }

            // Mevcut repository method'unu kullan
            var cartItem = await shoppingCartRepository.AddItem(cartItemToAddDto);

            if (cartItem == null)
            {
                logger.LogError("Failed to add item to cart - Product ID: {ProductId}", cartItemToAddDto.ProductId); 
                return null;
            }

            var product = await productRepository.GetItem(cartItem.ProductId);
            var result = cartItem.ConvertToDto(product);

            logger.LogInformation("Successfully added item to cart - Cart Item ID: {CartItemId}, Product: {ProductName}", result.Id, product.Name); // ← YENİ EKLENEN
            return result;
        }

        public async Task<CartItemDto> DeleteItem(int id)
        {
            logger.LogInformation("Deleting cart item with ID: {CartItemId}", id); 

            // Validation
            var validationResult = await cartItemValidator.ValidateDeleteItem(id);
            if (!validationResult.IsValid)
            {
                logger.LogWarning("Validation failed for deleting cart item: {ValidationError}", validationResult.ErrorMessage); 
                throw new ArgumentException(validationResult.ErrorMessage);
            }

            var cartItem = await shoppingCartRepository.DeleteItem(id);

            if (cartItem == null)
            {
                logger.LogWarning("Cart item not found for deletion - ID: {CartItemId}", id); 
                return null;
            }

            var product = await productRepository.GetItem(cartItem.ProductId);
            var result = cartItem.ConvertToDto(product);

            logger.LogInformation("Successfully deleted cart item - ID: {CartItemId}, Product: {ProductName}", id, product?.Name); 
            return result;
        }

        public async Task<CartItemDto> UpdateQty(CartItemQtyUpdateDto cartItemQtyUpdateDto)
        {
            logger.LogInformation("Updating cart item quantity - Cart Item ID: {CartItemId}, New Quantity: {Quantity}", cartItemQtyUpdateDto.CartItemId, cartItemQtyUpdateDto.Qty);

            /*
            // Business Logic: Miktar kontrolü
            if (cartItemQtyUpdateDto.Qty <= 0)
            {
                throw new ArgumentException("Quantity must be greater than 0");
            }
            */

            // Validation
            var validationResult = await cartItemValidator.ValidateUpdateQty(cartItemQtyUpdateDto);
            if (!validationResult.IsValid)
            {
                logger.LogWarning("Validation failed for updating cart item quantity: {ValidationError}", validationResult.ErrorMessage);
                throw new ArgumentException(validationResult.ErrorMessage);
            }

            var cartItem = await shoppingCartRepository.UpdateQty(cartItemQtyUpdateDto.CartItemId, cartItemQtyUpdateDto);

            if (cartItem == null)
            {
                logger.LogWarning("Cart item not found for quantity update - ID: {CartItemId}", cartItemQtyUpdateDto.CartItemId); 
                return null;
            }

            var product = await productRepository.GetItem(cartItem.ProductId);
            var result = cartItem.ConvertToDto(product);

            logger.LogInformation("Successfully updated cart item quantity - ID: {CartItemId}, Product: {ProductName}, New Quantity: {Quantity}", cartItemQtyUpdateDto.CartItemId, product?.Name, cartItemQtyUpdateDto.Qty); // ← YENİ EKLENEN
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
                logger.LogWarning("Product not found for cart item ID: {CartItemId}, Product ID: {ProductId}", id, cartItem.ProductId); 
                return null;
            }

            logger.LogInformation("Successfully retrieved cart item ID: {CartItemId}", id); 
            return cartItem.ConvertToDto(product);
        }
    }
}
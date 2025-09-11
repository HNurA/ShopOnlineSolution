using ShopOnline.Api.Repositories.Contracts;
using ShopOnline.Api.Services.Contracts;
using ShopOnline.Api.Extensions;
using ShopOnline.Models.Dtos;
using ShopOnline.Api.Entities;
using ShopOnline.Api.Validators.Contracts;

namespace ShopOnline.Api.Services
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly IShoppingCartRepository shoppingCartRepository;
        private readonly IProductRepository productRepository;
        private readonly ICartItemValidator cartItemValidator;

        public ShoppingCartService(IShoppingCartRepository shoppingCartRepository,
                                 IProductRepository productRepository,
                                 ICartItemValidator cartItemValidator)
        {
            this.shoppingCartRepository = shoppingCartRepository;
            this.productRepository = productRepository;
            this.cartItemValidator = cartItemValidator;
        }

        public async Task<List<CartItemDto>> GetItems(int userId)
        {
            var cartItems = await shoppingCartRepository.GetItems(userId);

            if (cartItems == null || !cartItems.Any())
            {
                return new List<CartItemDto>();
            }

            var products = await productRepository.GetItems();
            return cartItems.ConvertToDto(products).ToList();
        }

        public async Task<CartItemDto> AddItem(CartItemToAddDto cartItemToAddDto)
        {
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
                throw new ArgumentException(validationResult.ErrorMessage);
            }

            // Mevcut repository method'unu kullan
            var cartItem = await shoppingCartRepository.AddItem(cartItemToAddDto);

            if (cartItem == null)
            {
                return null;
            }

            var product = await productRepository.GetItem(cartItem.ProductId);
            return cartItem.ConvertToDto(product);
        }

        public async Task<CartItemDto> DeleteItem(int id)
        {
            // Validation
            var validationResult = await cartItemValidator.ValidateDeleteItem(id);
            if (!validationResult.IsValid)
            {
                throw new ArgumentException(validationResult.ErrorMessage);
            }

            var cartItem = await shoppingCartRepository.DeleteItem(id);

            if (cartItem == null)
            {
                return null;
            }

            var product = await productRepository.GetItem(cartItem.ProductId);
            return cartItem.ConvertToDto(product);
        }

        public async Task<CartItemDto> UpdateQty(CartItemQtyUpdateDto cartItemQtyUpdateDto)
        {
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
                throw new ArgumentException(validationResult.ErrorMessage);
            }

            var cartItem = await shoppingCartRepository.UpdateQty(cartItemQtyUpdateDto.CartItemId, cartItemQtyUpdateDto);

            if (cartItem == null)
            {
                return null;
            }

            var product = await productRepository.GetItem(cartItem.ProductId);
            return cartItem.ConvertToDto(product);
        }

        public async Task<CartItemDto> GetItem(int id)
        {
            var cartItem = await shoppingCartRepository.GetItem(id);

            if (cartItem == null)
            {
                return null;
            }

            var product = await productRepository.GetItem(cartItem.ProductId);

            if (product == null)
            {
                return null;
            }

            return cartItem.ConvertToDto(product);
        }
    }
}
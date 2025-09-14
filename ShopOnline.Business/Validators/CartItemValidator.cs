//Business Logic ve Validation ayrılmalı

using Microsoft.AspNetCore.Mvc;
using ShopOnline.DataAccess.Repositories.Contracts;
using ShopOnline.Business.Services.Contracts;
using ShopOnline.Models.Dtos;
using ShopOnline.Business.Validators.Contracts;

namespace ShopOnline.Business.Validators
{
    public class CartItemValidator : ICartItemValidator
    {
        private readonly IProductRepository productRepository;
        private readonly IShoppingCartRepository shoppingCartRepository;

        public CartItemValidator(IProductRepository productRepository,
                                 IShoppingCartRepository shoppingCartRepository)
        {
            this.productRepository = productRepository;
            this.shoppingCartRepository = shoppingCartRepository;
        }
        public async Task<(bool IsValid, string ErrorMessage)> ValidateAddItem(CartItemToAddDto cartItemToAddDto)
        {
            // null check
            if (cartItemToAddDto == null)               
                return (false, "Cart item data is required");
                
            // ProductId validation
            if (cartItemToAddDto.ProductId <= 0)               
                return (false, "Valid Product Id is required");

            // Quantity validation
            if (cartItemToAddDto.Qty <= 0)
                return (false, "Quantity must be greater than 0");

            // CartId validation
            if (cartItemToAddDto.CartId <= 0)
                return (false, "Valid CartId required");

            // Product exists validation
            var product = await productRepository.GetItem(cartItemToAddDto.ProductId);
            if (product == null)
                return (false, $"Product with ID {cartItemToAddDto.ProductId} not found");

            // Stock validation
            if (product.Qty < cartItemToAddDto.Qty)
                return (false, $"Insufficient stock. Available: {product.Qty}, Requested: {cartItemToAddDto.Qty}");

            return (true, string.Empty);
        }

        public async Task<(bool IsValid, string ErrorMessage)> ValidateDeleteItem(int cartItemId)
        {
            // ID validation
            if (cartItemId <= 0)
                return (false, "Valid Cart Item Id is requires");

            // Cart item exists validation
            var cartItem = await shoppingCartRepository.GetItem(cartItemId);
            if (cartItem == null)
                return (false, $"Cart item with ID {cartItemId} not found");

            return (true, string.Empty);
        }

        public async Task<(bool IsValid, string ErrorMessage)> ValidateUpdateQty(CartItemQtyUpdateDto cartItemQtyUpdateDto)
        {
            // null check
            if (cartItemQtyUpdateDto == null)
                return (false, "Update quantity data is required");

            // ProductId validation
            if (cartItemQtyUpdateDto.CartItemId <= 0)
                return (false, "Valid Cart Item ID is required");

            // Quantity validation
            if (cartItemQtyUpdateDto.Qty <= 0)
                return (false, "Quantity must be greater than 0");

            // Cart item exists validation
            var cartItem = await shoppingCartRepository.GetItem(cartItemQtyUpdateDto.CartItemId);
            if (cartItem == null)
                return (false, $"Cart item with ID {cartItemQtyUpdateDto.CartItemId} not found");

            // Product stock validation
            var product = await productRepository.GetItem(cartItem.ProductId);
            if (product != null && product.Qty < cartItemQtyUpdateDto.Qty)
                return (false, $"Insufficient stock. Available: {product.Qty}, Requested: {cartItemQtyUpdateDto.Qty}");

            return (true, string.Empty);
        }
    }
}

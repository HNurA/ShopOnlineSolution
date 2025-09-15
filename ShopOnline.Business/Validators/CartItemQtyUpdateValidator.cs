using FluentValidation;
using ShopOnline.Models.Dtos;
using ShopOnline.DataAccess.Repositories.Contracts;

namespace ShopOnline.Business.Validators
{
    public class CartItemQtyUpdateValidator : AbstractValidator<CartItemQtyUpdateDto>
    {
        private readonly IProductRepository _productRepository;
        private readonly IShoppingCartRepository _shoppingCartRepository;

        public CartItemQtyUpdateValidator(
            IProductRepository productRepository,
            IShoppingCartRepository shoppingCartRepository)
        {
            _productRepository = productRepository;
            _shoppingCartRepository = shoppingCartRepository;

            // Temel kurallar
            RuleFor(x => x.CartItemId)
                .GreaterThan(0)
                .WithMessage("Valid Cart Item ID is required");

            RuleFor(x => x.Qty)
                .GreaterThan(0)
                .WithMessage("Quantity must be greater than 0");

            // Async validations
            RuleFor(x => x.CartItemId)
                .MustAsync(async (cartItemId, cancellation) => await CartItemExists(cartItemId))
                .WithMessage(x => $"Cart item with ID {x.CartItemId} not found");

            RuleFor(x => x)
                .MustAsync(async (dto, cancellation) => await HasSufficientStockForUpdate(dto))
                .WithMessage("Insufficient stock for the requested quantity")
                .WithName("Stock");
        }

        private async Task<bool> CartItemExists(int cartItemId)
        {
            try
            {
                var cartItem = await _shoppingCartRepository.GetItem(cartItemId);
                return cartItem != null;
            }
            catch
            {
                return false;
            }
        }

        private async Task<bool> HasSufficientStockForUpdate(CartItemQtyUpdateDto dto)
        {
            try
            {
                var cartItem = await _shoppingCartRepository.GetItem(dto.CartItemId);
                if (cartItem == null) return false;

                var product = await _productRepository.GetItem(cartItem.ProductId);
                return product != null && product.Qty >= dto.Qty;
            }
            catch
            {
                return false;
            }
        }
    }
}
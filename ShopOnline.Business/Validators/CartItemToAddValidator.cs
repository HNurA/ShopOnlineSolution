using FluentValidation;
using ShopOnline.Models.Dtos;
using ShopOnline.DataAccess.Repositories.Contracts;

namespace ShopOnline.Business.Validators
{
    public class CartItemToAddValidator : AbstractValidator<CartItemToAddDto>
    {
        private readonly IProductRepository _productRepository;

        public CartItemToAddValidator(IProductRepository productRepository)
        {
            _productRepository = productRepository;

            // Temel kurallar
            RuleFor(x => x.ProductId)
                .GreaterThan(0)
                .WithMessage("Valid Product Id is required");

            RuleFor(x => x.Qty)
                .GreaterThan(0)
                .WithMessage("Quantity must be greater than 0");

            RuleFor(x => x.CartId)
                .GreaterThan(0)
                .WithMessage("Valid CartId required");

            // Async validations
            RuleFor(x => x.ProductId)
                .MustAsync(async (productId, cancellation) => await ProductExists(productId))
                .WithMessage(x => $"Product with ID {x.ProductId} not found");

            RuleFor(x => x)
                .MustAsync(async (cartItem, cancellation) => await HasSufficientStock(cartItem))
                .WithMessage(x => "Insufficient stock for the requested quantity")
                .WithName("Stock");
        }

        private async Task<bool> ProductExists(int productId)
        {
            try
            {
                var product = await _productRepository.GetItem(productId);
                return product != null;
            }
            catch
            {
                return false;
            }
        }

        private async Task<bool> HasSufficientStock(CartItemToAddDto cartItem)
        {
            try
            {
                var product = await _productRepository.GetItem(cartItem.ProductId);
                return product != null && product.Qty >= cartItem.Qty;
            }
            catch
            {
                return false;
            }
        }
    }
}
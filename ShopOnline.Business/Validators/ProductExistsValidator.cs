using FluentValidation;
using ShopOnline.DataAccess.Repositories.Contracts;

namespace ShopOnline.Business.Validators
{
    public class ProductExistsValidator : AbstractValidator<int>
    {
        private readonly IProductRepository _productRepository;

        public ProductExistsValidator(IProductRepository productRepository)
        {
            _productRepository = productRepository;

            RuleFor(productId => productId)
                .GreaterThan(0)
                .WithMessage("Valid Product ID is required");

            RuleFor(productId => productId)
                .MustAsync(async (productId, cancellation) => await ProductExists(productId))
                .WithMessage(productId => $"Product with ID {productId} not found");
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
    }
}
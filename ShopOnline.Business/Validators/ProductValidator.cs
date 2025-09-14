using ShopOnline.DataAccess.Repositories.Contracts;
using ShopOnline.Business.Validators.Contracts;

namespace ShopOnline.Business.Validators
{
    public class ProductValidator : IProductValidator
    {
        private readonly IProductRepository productRepository;

        public ProductValidator(IProductRepository productRepository)
        {
            this.productRepository = productRepository;
        }

        public async Task<(bool IsValid, string ErrorMessage)> ValidateCategoryExists(int categoryId)
        {
            // ID validation
            if (categoryId <= 0)
                return (false, "Valid Category ID is required");

            var categories = await productRepository.GetCategories();
            var categoryExists = categories?.Any(c => c.Id == categoryId) ?? false;

            if (!categoryExists)
                return (false, $"Category with ID {categoryId} not found");

            return (true, string.Empty);
        }

        public async Task<(bool IsValid, string ErrorMessage)> ValidateProductExists(int productId)
        {
            // ID validation
            if (productId <= 0)
                return (false, "Valid Product ID is required");

            var product = await productRepository.GetItem(productId);
            if (product == null)
                return (false, $"Product with ID {productId} not found");

            return (true, string.Empty);
        }
    }
}
using FluentValidation;
using ShopOnline.DataAccess.Repositories.Contracts;

namespace ShopOnline.Business.Validators
{
    public class CategoryExistsValidator : AbstractValidator<int>
    {
        private readonly IProductRepository _productRepository;

        public CategoryExistsValidator(IProductRepository productRepository)
        {
            _productRepository = productRepository;

            RuleFor(categoryId => categoryId)
                .GreaterThan(0)
                .WithMessage("Valid Category ID is required");

            RuleFor(categoryId => categoryId)
                .MustAsync(async (categoryId, cancellation) => await CategoryExists(categoryId))
                .WithMessage(categoryId => $"Category with ID {categoryId} not found");
        }

        private async Task<bool> CategoryExists(int categoryId)
        {
            try
            {
                var categories = await _productRepository.GetCategories();
                return categories?.Any(c => c.Id == categoryId) ?? false;
            }
            catch
            {
                return false;
            }
        }
    }
}
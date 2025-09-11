using ShopOnline.Api.Extensions;
using ShopOnline.Api.Repositories.Contracts;
using ShopOnline.Api.Services.Contracts;
using ShopOnline.Models.Dtos;

namespace ShopOnline.Api.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository productRepository;

        public ProductService(IProductRepository productRepository)
        {
            this.productRepository = productRepository;
        }

        public async Task<ProductDto> GetProduct(int id)
        {
            var product = await productRepository.GetItem(id);

            if (product == null)
            {
                return null;
            }

            return product.ConvertToDto();
        }

        public async Task<IEnumerable<ProductDto>> GetProductsByCategory(int categoryId)
        {
            var products = await productRepository.GetItemsByCategory(categoryId);

            if (products == null)
            {
                return Enumerable.Empty<ProductDto>();
            }

            return products.ConvertToDto();
        }

        public async Task<IEnumerable<ProductDto>> GetProducts()
        {
            var products = await productRepository.GetItems();

            if (products == null)
            {
                return Enumerable.Empty<ProductDto>();
            }

            return products.ConvertToDto();
        }

        public async Task<IEnumerable<ProductCategoryDto>> GetProductCategories()
        {
            var categories = await productRepository.GetCategories();

            if (categories == null)
            {
                return Enumerable.Empty<ProductCategoryDto>();
            }

            return categories.ConvertToDto();
        }
    }
}

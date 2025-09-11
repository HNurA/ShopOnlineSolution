using ShopOnline.Models.Dtos;

namespace ShopOnline.Api.Services.Contracts
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetProducts();
        Task<ProductDto> GetProduct(int id);
        Task<IEnumerable<ProductCategoryDto>> GetProductCategories();
        Task<IEnumerable<ProductDto>> GetProductsByCategory(int categoryId);
    }
}

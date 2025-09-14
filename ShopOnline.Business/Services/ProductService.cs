// ProductService.cs - Cache ile güncellenmiş versiyon
using ShopOnline.DataAccess.Repositories.Contracts;
using ShopOnline.Business.Services.Contracts;
using ShopOnline.Business.Extensions;
using ShopOnline.Models.Dtos;
using Microsoft.Extensions.Logging;

namespace ShopOnline.Business.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository productRepository;
        private readonly ILogger<ProductService> logger;
        private readonly ICacheService cacheService; 

        // Cache keys
        private const string ALL_PRODUCTS_CACHE_KEY = "all_products";
        private const string ALL_CATEGORIES_CACHE_KEY = "all_categories";
        private const string PRODUCT_CACHE_KEY_PREFIX = "product_";
        private const string CATEGORY_PRODUCTS_CACHE_KEY_PREFIX = "category_products_";

        public ProductService(IProductRepository productRepository,
                            ILogger<ProductService> logger,
                            ICacheService cacheService)
        {
            this.productRepository = productRepository;
            this.logger = logger;
            this.cacheService = cacheService;
        }

        public async Task<IEnumerable<ProductDto>> GetProducts()
        {
            logger.LogInformation("Getting all products started");

            // Cache'den kontrol et
            var cachedProducts = await cacheService.GetAsync<IEnumerable<ProductDto>>(ALL_PRODUCTS_CACHE_KEY);
            if (cachedProducts != null)
            {
                logger.LogInformation("Products retrieved from cache");
                return cachedProducts;
            }

            // Cache'de yoksa database'den al
            var products = await productRepository.GetItems();

            if (products == null)
            {
                logger.LogWarning("No products found in database");
                return Enumerable.Empty<ProductDto>();
            }

            var productDtos = products.ConvertToDto();

            // Cache'e kaydet
            await cacheService.SetAsync(ALL_PRODUCTS_CACHE_KEY, productDtos, TimeSpan.FromMinutes(30));

            logger.LogInformation("Successfully retrieved {ProductCount} products from database and cached", productDtos.Count());

            return productDtos;
        }

        public async Task<ProductDto> GetProduct(int id)
        {
            logger.LogInformation("Getting product with ID: {ProductId}", id);

            var cacheKey = $"{PRODUCT_CACHE_KEY_PREFIX}{id}";

            // Cache'den kontrol et
            var cachedProduct = await cacheService.GetAsync<ProductDto>(cacheKey);
            if (cachedProduct != null)
            {
                logger.LogInformation("Product retrieved from cache - ID: {ProductId}", id);
                return cachedProduct;
            }

            // Cache'de yoksa database'den al
            var product = await productRepository.GetItem(id);

            if (product == null)
            {
                logger.LogWarning("Product not found with ID: {ProductId}", id);
                return null;
            }

            var productDto = product.ConvertToDto();

            // Cache'e kaydet
            await cacheService.SetAsync(cacheKey, productDto, TimeSpan.FromMinutes(15));

            logger.LogInformation("Successfully retrieved product: {ProductName} (ID: {ProductId}) from database and cached", product.Name, id);
            return productDto;
        }

        public async Task<IEnumerable<ProductCategoryDto>> GetProductCategories()
        {
            logger.LogInformation("Getting all product categories started");

            // Cache'den kontrol et
            var cachedCategories = await cacheService.GetAsync<IEnumerable<ProductCategoryDto>>(ALL_CATEGORIES_CACHE_KEY);
            if (cachedCategories != null)
            {
                logger.LogInformation("Categories retrieved from cache");
                return cachedCategories;
            }

            // Cache'de yoksa database'den al
            var categories = await productRepository.GetCategories();

            if (categories == null)
            {
                logger.LogWarning("No categories found in database");
                return Enumerable.Empty<ProductCategoryDto>();
            }

            var categoryDtos = categories.ConvertToDto();

            // Cache'e kaydet
            await cacheService.SetAsync(ALL_CATEGORIES_CACHE_KEY, categoryDtos, TimeSpan.FromHours(1));

            logger.LogInformation("Successfully retrieved {CategoryCount} categories from database and cached", categoryDtos.Count());

            return categoryDtos;
        }

        public async Task<IEnumerable<ProductDto>> GetProductsByCategory(int categoryId)
        {
            logger.LogInformation("Getting products for category ID: {CategoryId}", categoryId);

            var cacheKey = $"{CATEGORY_PRODUCTS_CACHE_KEY_PREFIX}{categoryId}";

            // Cache'den kontrol et
            var cachedProducts = await cacheService.GetAsync<IEnumerable<ProductDto>>(cacheKey);
            if (cachedProducts != null)
            {
                logger.LogInformation("Category products retrieved from cache - Category ID: {CategoryId}", categoryId);
                return cachedProducts;
            }

            // Cache'de yoksa database'den al
            var products = await productRepository.GetItemsByCategory(categoryId);

            if (products == null)
            {
                logger.LogWarning("No products found for category ID: {CategoryId}", categoryId);
                return Enumerable.Empty<ProductDto>();
            }

            var productDtos = products.ConvertToDto();

            // Cache'e kaydet
            await cacheService.SetAsync(cacheKey, productDtos, TimeSpan.FromMinutes(20));

            logger.LogInformation("Successfully retrieved {ProductCount} products for category ID: {CategoryId} from database and cached",
                productDtos.Count(), categoryId);

            return productDtos;
        }
    }
}
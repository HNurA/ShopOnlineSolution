/* try-catch blokları gitti
** exception handling middleware de hepsi kontrol ediliyor */

using Microsoft.AspNetCore.Mvc;
using ShopOnline.Api.Entities;
using ShopOnline.Api.Services.Contracts;
using ShopOnline.Models.Dtos;

namespace ShopOnline.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService productService;

        public ProductController(IProductService productService)
        {
            this.productService = productService;
        }

        [HttpGet] 
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetItems()
        {
            var productDtos = await this.productService.GetProducts();

            if (productDtos == null || !productDtos.Any())
            {
                return NotFound("No products found");
            }

            return Ok(productDtos);
        }

        [HttpGet("{id:int}")] 
        public async Task<ActionResult<ProductDto>> GetItem(int id)
        {
            var productDto = await this.productService.GetProduct(id);

            if (productDto == null)
            {
                return NotFound($"Product with ID {id} not found");
            }
            
            return Ok(productDto);

        }

        [HttpGet]
        [Route(nameof(GetProductCategories))]
        public async Task<ActionResult<IEnumerable<ProductCategory>>> GetProductCategories()
        {
            var productCategoryDtos = await productService.GetProductCategories();

            if (productCategoryDtos == null || !productCategoryDtos.Any())
            {
                return NotFound();
            }

            return Ok(productCategoryDtos);
        }

        [HttpGet]
        [Route("{categoryId}/GetItemsByCategory")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetItemsByCategory(int categoryId)
        {
            var productDtos = await productService.GetProductsByCategory(categoryId);

            if (productDtos == null || !productDtos.Any())
            {
                return NotFound();
            }

            return Ok(productDtos);
        }
    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShopOnline.Api.Entities;
using ShopOnline.Api.Extensions;
using ShopOnline.Api.Repositories.Contracts;
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
            try
            {
                var productDtos = await this.productService.GetProducts();

                if (productDtos == null || !productDtos.Any())
                {
                    return NotFound();
                }

                return Ok(productDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                                  "Error retrieving data from the database!");
            }
        }

        [HttpGet("{id:int}")] 
        public async Task<ActionResult<ProductDto>> GetItem(int id)
        {
            try
            {
                var productDto = await this.productService.GetProduct(id);

                if (productDto == null)
                {
                    return BadRequest();
                }
                else
                {
                    return Ok(productDto);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                                 "Error retrieving data from the database!");
            }
        }

        [HttpGet]
        [Route(nameof(GetProductCategories))]
        public async Task<ActionResult<IEnumerable<ProductCategory>>> GetProductCategories()
        {
            try
            {
                var productCategoryDtos = await productService.GetProductCategories();

                if(productCategoryDtos == null || !productCategoryDtos.Any())
                { 
                    return NotFound(); 
                }

                return Ok(productCategoryDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                                  "Error retrieving data from the database");
            }
        }

        [HttpGet]
        [Route("{categoryId}/GetItemsByCategory")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetItemsByCategory(int categoryId)
        {
            try
            {
                var productDtos = await productService.GetProductsByCategory(categoryId);

                if (productDtos == null || !productDtos.Any())
                {
                    return NotFound();
                }

                return Ok(productDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                                  "Error retrieving data from the database");
            }
        }
    }
}

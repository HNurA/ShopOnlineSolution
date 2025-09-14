using Microsoft.AspNetCore.Mvc;
using ShopOnline.Business.Services.Contracts;
using ShopOnline.Models.Dtos;

namespace ShopOnline.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShoppingCartController : ControllerBase
    {
        private readonly IShoppingCartService shoppingCartService;

        public ShoppingCartController(IShoppingCartService shoppingCartService)
        {
            this.shoppingCartService = shoppingCartService;
        }

        [HttpGet]
        [Route("{userId}/GetItems")]
        public async Task<ActionResult<IEnumerable<CartItemDto>>> GetItems(int userId)
        {
            var cartItemDtos = await this.shoppingCartService.GetItems(userId);

            if (cartItemDtos == null || !cartItemDtos.Any())
            {
                return NoContent();
            }

            return Ok(cartItemDtos);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<CartItemDto>> GetItem(int id)
        {
            var cartItemDto = await this.shoppingCartService.GetItem(id);

            if (cartItemDto == null)
            {
                return NotFound();
            }

            return Ok(cartItemDto);
        }

        [HttpPost]
        public async Task<ActionResult<CartItemDto>> PostItem([FromBody] CartItemToAddDto cartItemToAddDto)
        {
            var newCartItemDto = await this.shoppingCartService.AddItem(cartItemToAddDto);

            if (newCartItemDto == null)
            {
                return NoContent();
            }

            return CreatedAtAction(nameof(GetItem), new { id = newCartItemDto.Id }, newCartItemDto);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<CartItemDto>> DeleteItem(int id)
        {
            var cartItemDto = await this.shoppingCartService.DeleteItem(id);
            if (cartItemDto == null)
            {
                return NotFound();
            }

            return Ok(cartItemDto);
        }

        [HttpPatch("{id:int}")]
        public async Task<ActionResult<CartItemDto>> UpdateQty(int id, CartItemQtyUpdateDto cartItemQtyUpdateDto)
        {
            cartItemQtyUpdateDto.CartItemId = id; // ID'yi DTO'ya set et
            var cartItemDto = await this.shoppingCartService.UpdateQty(cartItemQtyUpdateDto);

            if (cartItemDto == null)
            {
                return NotFound();
            }

            return Ok(cartItemDto);
        }
    }
}
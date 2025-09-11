using ShopOnline.Models.Dtos;

namespace ShopOnline.Api.Services.Contracts
{
    public interface IShoppingCartService
    {
        Task<List<CartItemDto>> GetItems(int userId);
        Task<CartItemDto> GetItem(int id);
        Task<CartItemDto> AddItem(CartItemToAddDto cartItemToAddDto);
        Task<CartItemDto> DeleteItem(int id);
        Task<CartItemDto> UpdateQty(CartItemQtyUpdateDto cartItemQtyUpdate);
    }
}

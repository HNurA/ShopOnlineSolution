using ShopOnline.Models.Dtos;

namespace ShopOnline.Validation.Validators.Contracts
{
    public interface ICartItemValidator
    {
        Task<(bool IsValid, string ErrorMessage)> ValidateAddItem(CartItemToAddDto cartItemToAddDto);
        Task<(bool IsValid, string ErrorMessage)> ValidateUpdateQty(CartItemQtyUpdateDto cartItemQtyUpdateDto);
        Task<(bool IsValid, string ErrorMessage)> ValidateDeleteItem(int cartItemId);
    }
}

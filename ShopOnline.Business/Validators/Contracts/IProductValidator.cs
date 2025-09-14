namespace ShopOnline.Business.Validators.Contracts
{
    public interface IProductValidator
    {
        Task<(bool IsValid, string ErrorMessage)> ValidateProductExists(int productId);
        Task<(bool IsValid, string ErrorMessage)> ValidateCategoryExists(int categoryId);
    }
}

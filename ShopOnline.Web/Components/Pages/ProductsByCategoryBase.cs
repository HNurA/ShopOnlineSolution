using Microsoft.AspNetCore.Components;
using ShopOnline.Models.Dtos;
using ShopOnline.Web.Services.Contracts;
using System.Threading.Tasks;

namespace ShopOnline.Web.Components.Pages
{
    public class ProductsByCategoryBase : ComponentBase
    {
        [Parameter]
        public int CategoryId { get; set; }

        [Inject]
        public IProductService ProductService { get; set; }

        [Inject]
        public IManageProductsLocalStorageService ManageProductsLocalStorageService { get; set; }

        public IEnumerable<ProductDto> Products { get; set; }
        public string CategoryName { get; set; }
        public string ErrorMessage { get; set; }
        protected bool isLoaded { get; set; } = false;

        protected override async Task OnParametersSetAsync()
        {
            // Parameter değiştiğinde yeniden yükle
            isLoaded = false;
            Products = null;
            CategoryName = null;
            ErrorMessage = null;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!isLoaded)
            {
                try
                {
                    Products = await GetProductCollectionByCategoryId(CategoryId);

                    if (Products != null && Products.Count() > 0)
                    {
                        var productDto = Products.FirstOrDefault(p => p.CategoryId == CategoryId);
                        if (productDto != null)
                        {
                            CategoryName = productDto.CategoryName;
                        }
                    }

                    isLoaded = true;
                    StateHasChanged();
                }
                catch (Exception ex)
                {
                    ErrorMessage = ex.Message;
                    isLoaded = true;
                    StateHasChanged();
                }
            }
        }

        private async Task<IEnumerable<ProductDto>> GetProductCollectionByCategoryId(int categoryId)
        {
            try
            {
                // Önce LocalStorage'dan dene
                var productCollection = await ManageProductsLocalStorageService.GetCollection();

                if (productCollection != null && productCollection.Any())
                {
                    return productCollection.Where(p => p.CategoryId == categoryId);
                }
                else
                {
                    // LocalStorage boşsa API'den al
                    return await ProductService.GetItemsByCategory(categoryId);
                }
            }
            catch (Exception)
            {
                // LocalStorage hatası varsa direkt API'den al
                return await ProductService.GetItemsByCategory(categoryId);
            }
        }
    }
}
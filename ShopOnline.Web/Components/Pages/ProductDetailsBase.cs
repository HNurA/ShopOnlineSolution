using Microsoft.AspNetCore.Components;
using ShopOnline.Models.Dtos;
using ShopOnline.Web.Services.Contracts;

namespace ShopOnline.Web.Components.Pages
{
    public class ProductDetailsBase:ComponentBase
    {
        [Parameter]
        public int Id { get; set; }

        [Inject]
        public IProductService ProductService { get; set; }

        [Inject]
        public IShoppingCartService ShoppingCartService { get; set; }

        [Inject]
        public IManageProductsLocalStorageService ManageProductsLocalStorageService { get; set; }

        [Inject]
        public IManageCartItemsLocalStorageService ManageCartItemsLocalStorageService { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        public ProductDto Product { get; set; }

        public string ErrorMessage {  get; set; }

        private List<CartItemDto> ShoppingCartItems { get; set; }

        /*OnInitializedAsync iki kez çalışıyor
        çünkü Blazor Server'da prerendering ve interactive rendering olmak üzere iki aşama var.
        Bu durumda OnAfterRenderAsync kullanmak daha uygun 
        protected override async Task OnInitializedAsync()
        {
            try
            {
                //await Task.Delay(3000);
                ShoppingCartItems = await ManageCartItemsLocalStorageService.GetCollection();
                Product = await ProductService.GetItem(Id);
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
        }*/

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                try
                {
                    ShoppingCartItems = await ManageCartItemsLocalStorageService.GetCollection();
                    Product = await GetProductById(Id);
                    StateHasChanged(); // UI'ı güncelle
                }
                catch (Exception ex)
                {
                    ErrorMessage = ex.Message;
                    StateHasChanged(); // Hata durumunda da UI'ı güncelle
                }
            }
        }

        public async Task AddToCart_Click(CartItemToAddDto cartItemToAddDto)
        {
            try
            {
                var cartItemDto = await ShoppingCartService.AddItem(cartItemToAddDto);

                if (cartItemDto != null)
                {
                    ShoppingCartItems.Add(cartItemDto);
                    await ManageCartItemsLocalStorageService.SaveCollection(ShoppingCartItems);
                }

                // *** EKLENEN: Item eklendikten sonra cart changed event'ini tetikle ***
                await UpdateCartCount();

                NavigationManager.NavigateTo("/ShoppingCart");
            }
            catch (Exception)
            {
                //Log Exception
            }
        }

        private async Task<ProductDto> GetProductById(int id)
        {
            var productDtos = await ManageProductsLocalStorageService.GetCollection();

            if(productDtos != null)
            {
                return productDtos.SingleOrDefault(p => p.Id == id);
            }
            return null;
        }

        // *** EKLENEN: Cart count'u güncellemek için yeni method ***
        private async Task UpdateCartCount()
        {
            try
            {
                var cartItems = await ShoppingCartService.GetItems(HardCoded.UserId);
                int totalQuantity = cartItems?.Sum(item => item.Qty) ?? 0;

                // Cart changed event'ini tetikle
                ShoppingCartService.RaiseEventOnShoppingCartChanged(totalQuantity);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Cart count güncellenirken hata: {ex.Message}");
            }
        }

    }
}

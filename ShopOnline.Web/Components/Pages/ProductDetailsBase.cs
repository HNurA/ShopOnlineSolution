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
        public NavigationManager NavigationManager { get; set; }

        public ProductDto Product { get; set; }

        public string ErrorMessage {  get; set; }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                await Task.Delay(3000);
                Product = await ProductService.GetItem(Id);
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
        }

        public async Task AddToCart_Click(CartItemToAddDto cartItemToAddDto)
        {
            try
            {
                var cartItemDto = await ShoppingCartService.AddItem(cartItemToAddDto);

                // *** EKLENEN: Item eklendikten sonra cart changed event'ini tetikle ***
                await UpdateCartCount();

                NavigationManager.NavigateTo("/ShoppingCart");
            }
            catch (Exception)
            {
                //Log Exception
            }
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

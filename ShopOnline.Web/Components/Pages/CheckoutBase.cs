using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using ShopOnline.Models.Dtos;
using ShopOnline.Web.Services.Contracts;

namespace ShopOnline.Web.Components.Pages
{
    public class CheckoutBase : ComponentBase
    {
        [Inject]
        public IJSRuntime Js { get; set; }

        [Inject]
        public IShoppingCartService ShoppingCartService { get; set; }

        [Inject]
        public IManageCartItemsLocalStorageService ManageCartItemsLocalStorageService { get; set; }

        protected IEnumerable<CartItemDto> ShoppingCartItems { get; set; }
        protected int TotalQty { get; set; }
        protected string PaymentDescription { get; set; }
        protected decimal PaymentAmount { get; set; }
        protected bool isLoaded { get; set; } = false;
        protected string ErrorMessage { get; set; }

        protected override async Task OnInitializedAsync()
        {
            // LocalStorage çağrısı yok - boş bırak
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender && !isLoaded)
            {
                try
                {
                    ShoppingCartItems = await ManageCartItemsLocalStorageService.GetCollection();

                    if (ShoppingCartItems != null)
                    {
                        Guid orderGuid = Guid.NewGuid();
                        PaymentAmount = ShoppingCartItems.Sum(p => p.TotalPrice);
                        TotalQty = ShoppingCartItems.Sum(p => p.Qty);
                        PaymentDescription = $"O_{HardCoded.UserId}_{orderGuid}";
                    }

                    isLoaded = true;
                    StateHasChanged();

                    await Task.Delay(200);
                    await Js.InvokeVoidAsync("initPayPalButton");
                }
                catch (Exception ex)
                {
                    ErrorMessage = ex.Message;
                    isLoaded = true;
                    StateHasChanged();
                }
            }
        }
    }
}
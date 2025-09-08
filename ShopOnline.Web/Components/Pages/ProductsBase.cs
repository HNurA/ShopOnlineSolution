using Microsoft.AspNetCore.Components;
using ShopOnline.Models.Dtos;
using ShopOnline.Web.Services.Contracts;

namespace ShopOnline.Web.Components.Pages
{
    public class ProductsBase : ComponentBase
    {
        [Inject]
        public IProductService ProductService { get; set; } = default!;

        public IEnumerable<ProductDto> Products { get; set; } = new List<ProductDto>();
        public bool IsLoading { get; set; } = true; //loading kısmını getirebilmek için

        protected override async Task OnInitializedAsync()
        {
            IsLoading = true; 
            StateHasChanged(); 

            try
            {
                Products = await ProductService.GetItems();
            }
            finally
            {
                IsLoading = false; 
                StateHasChanged(); 
            }
        }

        protected IOrderedEnumerable<IGrouping<int, ProductDto>> GetGroupedProductsByCategory()
        {
            return from product in Products
                   group product by product.CategoryId into prodByCatGroup
                   orderby prodByCatGroup.Key
                   select prodByCatGroup;
        }

        protected string GetCategoryName(IGrouping<int, ProductDto> groupedProductDtos)
        {
            return groupedProductDtos.FirstOrDefault(pg => pg.CategoryId == groupedProductDtos.Key).CategoryName;
        }
    }
}
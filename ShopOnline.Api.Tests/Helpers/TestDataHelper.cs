// Helpers/TestDataHelper.cs
using ShopOnline.DataAccess.Entities;
using ShopOnline.Models.Dtos;

namespace ShopOnline.Api.Tests.Helpers
{
    public static class TestDataHelper
    {
        public static List<Product> GetTestProducts()
        {
            return new List<Product>
            {
                new Product
                {
                    Id = 1,
                    Name = "Test Product 1",
                    Description = "Test Description 1",
                    Price = 10.99m,
                    Qty = 100,
                    CategoryId = 1,
                    ImageURL = "test1.jpg",
                    ProductCategory = new ProductCategory { Id = 1, Name = "Electronics", IconCSS = "fas fa-laptop" }
                },
                new Product
                {
                    Id = 2,
                    Name = "Test Product 2",
                    Description = "Test Description 2",
                    Price = 20.99m,
                    Qty = 50,
                    CategoryId = 1,
                    ImageURL = "test2.jpg",
                    ProductCategory = new ProductCategory { Id = 1, Name = "Electronics", IconCSS = "fas fa-laptop" }
                }
            };
        }

        public static List<ProductCategory> GetTestCategories()
        {
            return new List<ProductCategory>
            {
                new ProductCategory { Id = 1, Name = "Electronics", IconCSS = "fas fa-laptop" },
                new ProductCategory { Id = 2, Name = "Clothing", IconCSS = "fas fa-tshirt" }
            };
        }

        public static List<CartItem> GetTestCartItems()
        {
            return new List<CartItem>
            {
                new CartItem { Id = 1, CartId = 1, ProductId = 1, Qty = 2 },
                new CartItem { Id = 2, CartId = 1, ProductId = 2, Qty = 1 }
            };
        }

        public static CartItemToAddDto GetTestCartItemToAddDto()
        {
            return new CartItemToAddDto
            {
                CartId = 1,
                ProductId = 1,
                Qty = 2
            };
        }

        public static CartItemQtyUpdateDto GetTestCartItemQtyUpdateDto()
        {
            return new CartItemQtyUpdateDto
            {
                CartItemId = 1,
                Qty = 3
            };
        }
    }
}
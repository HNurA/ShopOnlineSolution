using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Serilog;
using ShopOnline.Business.Services;
using ShopOnline.Business.Services.Contracts;
using ShopOnline.Business.Validators;
using ShopOnline.Business.Validators.Contracts;
using ShopOnline.DataAccess.Data;
using ShopOnline.DataAccess.Repositories;
using ShopOnline.DataAccess.Repositories.Contracts;
using ShopOnline.Models.Dtos;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database
builder.Services.AddDbContext<ShopOnlineDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ShopOnlineConnection")));

// Repository kayıtları
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IShoppingCartRepository, ShoppingCartRepository>();

// Cache kayıtları
builder.Services.AddMemoryCache();
builder.Services.AddScoped<ICacheService, CacheService>();

// FluentValidation kayıtları
builder.Services.AddScoped<IValidator<CartItemToAddDto>, CartItemToAddValidator>();
builder.Services.AddScoped<IValidator<CartItemQtyUpdateDto>, CartItemQtyUpdateValidator>();
builder.Services.AddScoped<IValidator<int>, ProductExistsValidator>();
builder.Services.AddScoped<IValidator<int>, CategoryExistsValidator>();
builder.Services.AddScoped<IValidator<int>, CartItemDeleteValidator>();

// Named validators for int (farklı amaçlar için)
builder.Services.AddScoped<IValidator<int>>(provider =>
    new ProductExistsValidator(provider.GetRequiredService<IProductRepository>()));

// Eğer category validator da gerekiyorsa:
// builder.Services.AddScoped<IValidator<int>>(provider => 
//     new CategoryExistsValidator(provider.GetRequiredService<IProductRepository>()));

builder.Services.AddScoped<IValidator<int>>(provider =>
    new CartItemDeleteValidator(provider.GetRequiredService<IShoppingCartRepository>()));

// Service kayıtları (Cache'den sonra olmalı!)
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IShoppingCartService, ShoppingCartService>();

// Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

var app = builder.Build();

// Configure the pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ShopOnline.Api.Middleware.ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthorization();
app.MapControllers();

app.Run();
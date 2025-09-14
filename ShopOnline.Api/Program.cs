using Microsoft.EntityFrameworkCore;
using Serilog;
using ShopOnline.DataAccess.Data;
using ShopOnline.DataAccess.Repositories;
using ShopOnline.DataAccess.Repositories.Contracts;
using ShopOnline.Business.Services;
using ShopOnline.Business.Services.Contracts;
using ShopOnline.Business.Validators;
using ShopOnline.Business.Validators.Contracts;

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

// ⭐ CACHE KAYITLARI - BU SATIRLARI EKLEYİN ⭐
builder.Services.AddMemoryCache();
builder.Services.AddScoped<ICacheService, CacheService>();

// Service kayıtları (Cache'den sonra olmalı!)
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IShoppingCartService, ShoppingCartService>();

// Validator kayıtları
builder.Services.AddScoped<ICartItemValidator, CartItemValidator>();
builder.Services.AddScoped<IProductValidator, ProductValidator>();

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

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthorization();
app.MapControllers();

app.Run();
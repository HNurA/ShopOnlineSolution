using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using ShopOnline.Api.Data;
using ShopOnline.Api.Repositories;
using ShopOnline.Api.Repositories.Contracts;
using ShopOnline.Api.Services;
using ShopOnline.Api.Services.Contracts;
using ShopOnline.Api.Validators;
using ShopOnline.Api.Validators.Contracts;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
// Swagger is an interface description language for describing restful apis expressed using json
builder.Services.AddSwaggerGen();

builder.Services.AddDbContextPool<ShopOnlineDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("ShopOnlineConnection"))
    );

builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IShoppingCartRepository, ShoppingCartRepository>();

// Yeni: Service kayıtları
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IShoppingCartService, ShoppingCartService>();

// Yeni: Validator kayıtları
builder.Services.AddScoped<IProductValidator, ProductValidator>();
builder.Services.AddScoped<ICartItemValidator, CartItemValidator>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(policy =>
    policy.WithOrigins("https://localhost:7182", "http://localhost:7182") 
    .AllowAnyMethod()
    .WithHeaders(HeaderNames.ContentType)
);

// Exception Handling Middleware 
app.UseMiddleware<ShopOnline.Api.Middleware.ExceptionHandlingMiddleware>();

// Mevcut middleware'ler
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

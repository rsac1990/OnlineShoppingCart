using Microsoft.EntityFrameworkCore;
using Mongo2Go;
using OnlineShopping.Api.Interfaces;
using OnlineShopping.Api.Services;
using OnlineShopping.Data.Catalog;
using OnlineShopping.Data.Catalog.Repositories;
using OnlineShopping.Domain.Cart.Business.Interfaces;
using OnlineShopping.Domain.Cart.Data;
using OnlineShopping.Domain.Cart.Data.Repositories;
using OnlineShopping.Domain.Catalog.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 1. Register your Catalog AppDbContext (replace "DefaultConnection" with your actual connection string name from appsettings.json)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("OnlineShoppingCatalogDb"));

// 1. Start the ephemeral MongoDB runner for your practice app
// 1. Start the ephemeral MongoDB runner
var runner = MongoDbRunner.Start();

// 2. Get the singular connection string property
var mongoConnectionString = runner.ConnectionString;
var databaseName = "OnlineShoppingCarts";

// 3. Register your MongoDB DBContext as a Singleton
builder.Services.AddSingleton<DBContext>(sp =>
    new DBContext(mongoConnectionString, databaseName));


// 1. Register your generic repository (Scoped lifetime is standard for EF Core / repositories)
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

// 2. Register your specific repositories (if you have custom ones like ICartRepository)
builder.Services.AddScoped<ICartRepository, CartRepository>();

builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICartService, CartService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

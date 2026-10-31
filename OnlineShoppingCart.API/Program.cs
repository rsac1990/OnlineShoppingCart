using Microsoft.Data.Sqlite;
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

// Create and open a persistent SQLite in-memory connection
var sqliteConnection = new SqliteConnection("DataSource=:memory:");
sqliteConnection.Open();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(sqliteConnection));

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

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.EnsureCreated();
}

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

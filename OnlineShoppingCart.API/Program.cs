using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Mongo2Go;
using OnlineShopping.Api;
using OnlineShopping.Api.Interfaces;
using OnlineShopping.Api.Services;
using OnlineShopping.Data.Catalog;
using OnlineShopping.Data.Catalog.Repositories;
using OnlineShopping.Domain.Cart.Business.Interfaces;
using OnlineShopping.Domain.Cart.Data;
using OnlineShopping.Domain.Cart.Data.Repositories;
using OnlineShopping.Domain.Catalog.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true; // Adds 'api-supported-versions' header to responses
    options.ApiVersionReader = new UrlSegmentApiVersionReader(); // Enforces URL segment versioning
})
.AddMvc()
.AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV"; // Formats groups as 'v1', 'v2', etc., for OpenAPI/Swagger
    options.SubstituteApiVersionInUrl = true;
});


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();

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
builder.Services.AddScoped<IProductRepository, ProductRepository>();

// 2. Register your specific repositories (if you have custom ones like ICartRepository)
builder.Services.AddScoped<ICartRepository, CartRepository>();

builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICartService, CartService>();

builder.Services.AddSingleton<IMessageProducer, RabbitMQMessageProducer>();

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
    app.UseSwaggerUI(options =>
    {
        // Obtener el proveedor de descripciones de versión creado por el ApiExplorer
        var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

        // Crear dinámicamente un endpoint en el menú desplegable de Swagger por cada versión detectada
        foreach (var description in provider.ApiVersionDescriptions)
        {
            options.SwaggerEndpoint(
                $"/swagger/{description.GroupName}/swagger.json",
                description.GroupName.ToUpperInvariant()
            );
        }
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

using Core;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using ProductService;
using ProductService.Categories;
using ProductService.Categories.Routing;
using ProductService.Categories.Service;
using ProductService.Categories.Storage;
using ProductService.Categories.Validation;
using ProductService.Products;
using ProductService.Products.Routing;
using ProductService.Products.Service;
using ProductService.Products.Storage;
using ProductService.Products.Validation;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder
    .Services //TODO verify db filepath + pass as ENV var if possible
    .AddDbContext<ProductDbContext>(options => options.UseSqlite("Data Source=Products.db"));

builder.Services
    .AddScoped<IBaseValidator<Category>, CategoryValidator>()
    .AddScoped<IBaseValidator<Product>, ProductValidator>()
    .AddScoped<ICrudRepository<Category>,CategoryRepository>()
    .AddScoped<ICrudRepository<Product>, ProductRepository>()
    .AddScoped<ICrudHandler<Category>, CategoryCrudHandler>()
    .AddScoped<ICrudHandler<Product>, ProductCrudHandler>()    
    ;

var app = builder.Build();

// migrate the database (just to simplify the startup of this sample app)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ProductDbContext>();
    await db.Database.MigrateAsync();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapCategoryEndpoints();
app.MapProductEndpoints();

app.Run();

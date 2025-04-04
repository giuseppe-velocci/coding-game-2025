using Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using ProductService;
using ProductService.Categories;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder
    .Services //TODO verify db filepath
    .AddDbContext<ProductDbContext>(options => options.UseSqlite("Data Source=bin/Debug/net8.0/Products.db"));

builder.Services.AddScoped<ICrudRepository<Category>, CategoryRepository>();

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
else
{
//    app.UseHttpsRedirection();
}

app.MapCategoryEndpoints();

app.Run();

using Core;
using Infrastructure;
using OrderApiGate;
using OrderApiGate.Addresses;
using OrderApiGate.Addresses.Routing;
using OrderApiGate.Addresses.Service;
using OrderApiGate.Categories;
using OrderApiGate.Categories.Routing;
using OrderApiGate.Categories.Service;
using OrderApiGate.Products;
using OrderApiGate.Products.Routing;
using OrderApiGate.Products.Service;
using OrderApiGate.Users;
using OrderApiGate.Users.Routing;
using OrderApiGate.Users.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

ApiGateConfig config = new()
{
    UserApiEndpoint = ApiGateConfig.BuildUrl(
        Environment.GetEnvironmentVariable($"USERS_ENDPOINT"),
        Environment.GetEnvironmentVariable($"ENDPOINTS_PORT")),
    AddressApiEndpoint = ApiGateConfig.BuildUrl(
        Environment.GetEnvironmentVariable($"ADDRESS_ENDPOINT"),
        Environment.GetEnvironmentVariable($"ENDPOINTS_PORT")),
    ProductApiEndpoint = ApiGateConfig.BuildUrl(
        Environment.GetEnvironmentVariable($"PRODUCT_ENDPOINT"),
        Environment.GetEnvironmentVariable($"ENDPOINTS_PORT")),
    OrderApiEndpoint = ApiGateConfig.BuildUrl(
        Environment.GetEnvironmentVariable($"ORDER_ENDPOINT"),
        Environment.GetEnvironmentVariable($"ENDPOINTS_PORT"))
};

builder.Services
    .AddHttpClient()
    .AddSingleton(config)
    .AddScoped<ICrudHandler<Address, WriteAddress>, AddressApiHandler>()
    .AddScoped<ICrudHandler<User, WriteUser>, UserApiHandler>()
    .AddScoped<ICrudHandler<Product, WriteProduct>, ProductApiHandler>()
    .AddScoped<ICrudHandler<Category, WriteCategory>, CategoryApiHandler>()
    ;

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapAddressEndpoints();
app.MapUserEndpoints();
app.MapProductEndpoints();
app.MapCategoryEndpoints();

app.Run();

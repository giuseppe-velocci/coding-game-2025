using Core;
using Infrastructure;
using OrderApiGate;
using OrderApiGate.Addresses;
using OrderApiGate.Addresses.Routing;
using OrderApiGate.Addresses.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

ApiGateConfig config = new()
{
    AddressApiEndpoint = ApiGateConfig.BuildUrl(
        Environment.GetEnvironmentVariable($"ADDRESS_ENDPOINT"),
        Environment.GetEnvironmentVariable($"ENDPOINTS_PORT"))
};

builder.Services
    .AddHttpClient()
    .AddSingleton(config)
    .AddScoped<ICrudHandler<Address, WriteAddress>, AddressApiHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapAddressEndpoints();

app.Run();

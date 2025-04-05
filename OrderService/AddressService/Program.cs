using AddressService;
using AddressService.Addresses;
using AddressService.Addresses.Routing;
using AddressService.Addresses.Service;
using AddressService.Addresses.Storage;
using AddressService.Addresses.Validation;
using Core;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder
    .Services //TODO verify db filepath + pass as ENV var if possible
    .AddDbContext<AddressDbContext>(options => options.UseSqlite("Data Source=Addresses.db"));

builder.Services
    .AddScoped<IBaseValidator<Address>, AddressValidator>()
    .AddScoped<ICrudRepository<Address>, AddressRepository>()
    .AddScoped<ICrudHandler<Address>, AddressCrudHandler>()
;
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapAddressEndpoints();

app.Run();
using AddressService;
using AddressService.Addresses;
using AddressService.Addresses.Routing;
using AddressService.Addresses.Service;
using AddressService.Addresses.Storage;
using AddressService.Addresses.Validation;
using Core;
using Infrastructure;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.Configure<RouteHandlerOptions>(options =>
 {
     options.ThrowOnBadRequest = true;
 });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder
    .Services
    .AddDbContext<AddressDbContext>(options => options.UseSqlite("Data Source=Addresses.db"));

builder.Services
    .AddScoped<IBaseValidator<Address>, AddressValidator>()
    .AddScoped<ICrudRepository<Address>, AddressRepository<SqliteException>>()
    .AddScoped<ICrudHandler<Address>, AddressCrudHandler>()
;
var app = builder.Build();

// Exception handling middleware
app.RunWithExceptionHandler();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // migrate the database (just to simplify the startup of this sample app)
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<AddressDbContext>();
        await db.Database.MigrateAsync();
    }

    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapAddressEndpoints();

app.Run();

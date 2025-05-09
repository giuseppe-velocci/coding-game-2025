using Core;
using Infrastructure;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using OrderService;
using OrderService.OrderRequests;
using OrderService.OrderRequests.Routing;
using OrderService.OrderRequests.Service;
using OrderService.OrderRequests.Validation;
using OrderService.Orders;
using OrderService.Orders.Storage;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder
    .Services
    .AddDbContext<OrderDbContext>(options => options.UseSqlite("Data Source=Orders.db"));

ApiEndpointConfig config = new(
    Environment.GetEnvironmentVariable("API_GATE_ENDPOINT"),
    Environment.GetEnvironmentVariable("ENDPOINTS_PORT")
);

builder.Services.AddHttpClient()
    .AddSingleton<ApiEndpointConfig>(config)
    .AddScoped<IBaseValidator<OrderRequest>, OrderRequestValidator>()
    .AddScoped<ICrudRepository<Order>, OrderRepository<SqliteException>>()
    .AddScoped<IApiGatewayCaller, ApiGatewayCaller>()
    .AddScoped<IHttpClientService, HttpApiClientService>()
    .AddScoped<IOrderRequestHandler, OrderRequestHandler>();

var app = builder.Build();

app.RunWithExceptionHandler();

app.MapOrderRequestEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // migrate the database (just to simplify the startup of this sample app)
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
        await db.Database.MigrateAsync();
    }

    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();

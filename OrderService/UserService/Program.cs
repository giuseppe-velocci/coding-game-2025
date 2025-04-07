using Core;
using Infrastructure;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using UserService;
using UserService.Users;
using UserService.Users.Routing;
using UserService.Users.Service;
using UserService.Users.Storage;
using UserService.Users.Validation;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder
    .Services
    .AddDbContext<UserDbContext>(options => options.UseSqlite("Data Source=Users.db"));

builder.Services
    .AddScoped<IBaseValidator<User>, UserValidator>()
    .AddScoped<ICrudRepository<User>, UserRepository<SqliteException>>()
    .AddScoped<ICrudHandler<User>, UserCrudHandler>();

var app = builder.Build();

app.RunWithExceptionHandler();

app.MapUserEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // migrate the database (just to simplify the startup of this sample app)
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<UserDbContext>();
        await db.Database.MigrateAsync();
    }
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();

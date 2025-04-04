using Core;
using Microsoft.AspNetCore.Mvc;

namespace ProductService.Products
{
    public static class ProductApiRouting
    {
        // CREATE a new Product
        public static IEndpointRouteBuilder MapProductEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapPost("/products", async ([FromBody] Product Product, ICrudRepository<Product> repo) =>
                {
                    await repo.Create(Product);
                    return Results.Created($"/products/{Product.CategoryId}", Product);
                });
            app.MapGet("/products", async (ICrudRepository<Product> repo) => await repo.ReadAll());
            app.MapGet("/products/{id:long}", async (long id, ICrudRepository<Product> repo) =>
            {
                var Product = await repo.ReadOne(id);
                return Product is not null ?
                    Results.Ok(Product) :
                    Results.NotFound($"Product with ID {id} not found.");
            });
            app.MapPut("/products/{id:long}", async (long id, [FromBody] Product updatedCategory, ICrudRepository<Product> repo) =>
            {
                await repo.Update(id, updatedCategory);
                return Results.Ok();
            });
            app.MapDelete("/products/{id:int}", async (int id, ICrudRepository<Product> repo) =>
            {
                await repo.Delete(id);
                return Results.NoContent();
            });

            return app;
        }
    }
}

using Core;
using Microsoft.AspNetCore.Mvc;

namespace ProductService.Products
{
    public static class ProductApiRouting
    {
        // CREATE a new Product
        public static IEndpointRouteBuilder MapProductEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapPost("/products", async (
                [FromBody] Product Product,
                ICrudRepository<Product> repo,
                CancellationToken cts) =>
                {
                    await repo.Create(Product, cts);
                    return Results.Created($"/products/{Product.CategoryId}", Product);
                });

            app.MapGet("/products", async (
                ICrudRepository<Product> repo,
                CancellationToken cts) => await repo.ReadAll(cts));

            app.MapGet("/products/{id:long}", async (long id, ICrudRepository<Product> repo, CancellationToken cts) =>
            {
                var Product = await repo.ReadOne(id, cts);
                return Product is not null ?
                    Results.Ok(Product) :
                    Results.NotFound($"Product with ID {id} not found.");
            });

            app.MapPut("/products/{id:long}", async (
                long id,
                [FromBody] Product updatedCategory,
                ICrudRepository<Product> repo,
                CancellationToken cts) =>
            {
                await repo.Update(id, updatedCategory, cts);
                return Results.Ok();
            });

            app.MapDelete("/products/{id:int}", async (
                int id,
                ICrudRepository<Product> repo,
                CancellationToken cts) =>
            {
                await repo.Delete(id, cts);
                return Results.NoContent();
            });

            return app;
        }
    }
}

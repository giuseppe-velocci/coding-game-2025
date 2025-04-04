using Core;
using Microsoft.AspNetCore.Mvc;

namespace ProductService.Products
{
    public static class ProductApiRouting
    {
        public static IEndpointRouteBuilder MapProductEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapPost("/products", async (
                [FromBody] Product Product,
                ICrudHandler<Product> repo,
                CancellationToken cts) =>
                {
                    await repo.Create(Product, cts);
                    return Results.Created($"/products/{Product.CategoryId}", Product);
                });

            app.MapGet("/products", async (
                ICrudHandler<Product> repo,
                CancellationToken cts) => await repo.ReadAll(cts));

            app.MapGet("/products/{id:long}", async (long id, ICrudHandler<Product> repo, CancellationToken cts) =>
            {
                var Product = await repo.ReadOne(id, cts);
                return Product is not null ?
                    Results.Ok(Product) :
                    Results.NotFound($"Product with ID {id} not found.");
            });

            app.MapPut("/products/{id:long}", async (
                long id,
                [FromBody] Product updatedCategory,
                ICrudHandler<Product> repo,
                CancellationToken cts) =>
            {
                await repo.Update(id, updatedCategory, cts);
                return Results.Ok();
            });

            app.MapDelete("/products/{id:int}", async (
                int id,
                ICrudHandler<Product> repo,
                CancellationToken cts) =>
            {
                await repo.Delete(id, cts);
                return Results.NoContent();
            });

            return app;
        }
    }
}

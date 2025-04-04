using Core;
using Microsoft.AspNetCore.Mvc;

namespace ProductService.Categories
{
    public static class ProductApiRouting
    {
        // CREATE a new category
        public static IEndpointRouteBuilder MapCategoryEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapPost("/categories", async (
                [FromBody] Category category,
                ICrudRepository<Category> repo,
                CancellationToken cts) =>
                {
                    await repo.Create(category, cts);
                    return Results.Created($"/categories/{category.CategoryId}", category);
                });

            app.MapGet("/categories", async (
                ICrudRepository<Category> repo,
                CancellationToken cts) => await repo.ReadAll(cts));

            app.MapGet("/categories/{id:long}", async (
                long id,
                ICrudRepository<Category> repo,
                CancellationToken cts) =>
            {
                var category = await repo.ReadOne(id, cts);
                return category is not null ?
                    Results.Ok(category) :
                    Results.NotFound($"Category with ID {id} not found.");
            });

            app.MapPut("/categories/{id:long}", async (
                long id,
                [FromBody] Category updatedCategory,
                ICrudRepository<Category> repo,
                CancellationToken cts) =>
            {
                await repo.Update(id, updatedCategory, cts);
                return Results.Ok();
            });

            app.MapDelete("/categories/{id:int}", async (
                int id,
                ICrudRepository<Category> repo,
                CancellationToken cts) =>
            {
                await repo.Delete(id, cts);
                return Results.NoContent();
            });

            return app;
        }
    }
}

using Core;
using Microsoft.AspNetCore.Mvc;

namespace ProductService.Categories
{
    public static class ProductApiRouting
    {
        public static IEndpointRouteBuilder MapCategoryEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapPost("/categories", async (
                [FromBody] Category category,
                ICrudHandler<Category> repo,
                CancellationToken cts) =>
                {
                    await repo.Create(category, cts);
                    return Results.Created($"/categories/{category.CategoryId}", category);
                });

            app.MapGet("/categories", async (
                ICrudHandler<Category> repo,
                CancellationToken cts) => await repo.ReadAll(cts));

            app.MapGet("/categories/{id:long}", async (
                long id,
                ICrudHandler<Category> repo,
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
                ICrudHandler<Category> repo,
                CancellationToken cts) =>
            {
                await repo.Update(id, updatedCategory, cts);
                return Results.Ok();
            });

            app.MapDelete("/categories/{id:int}", async (
                int id,
                ICrudHandler<Category> repo,
                CancellationToken cts) =>
            {
                await repo.Delete(id, cts);
                return Results.NoContent();
            });

            return app;
        }
    }
}

using Core;
using Microsoft.AspNetCore.Mvc;

namespace ProductService.Categories
{
    public static class CategoryApiRouting
    {
        // CREATE a new category
        public static IEndpointRouteBuilder MapCategoryEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapPost("/categories", async ([FromBody] Category category, ICrudRepository<Category> repo) =>
                {
                    await repo.Create(category);
                    return Results.Created($"/categories/{category.CategoryId}", category);
                });
            app.MapGet("/categories", async (ICrudRepository<Category> repo) => await repo.ReadAll());
            app.MapGet("/categories/{id:long}", async (long id, ICrudRepository<Category> repo) =>
            {
                var category = await repo.ReadOne(id);
                return category is not null ?
                    Results.Ok(category) :
                    Results.NotFound($"Category with ID {id} not found.");
            });
            app.MapPut("/categories/{id:long}", async (long id, [FromBody] Category updatedCategory, ICrudRepository<Category> repo) =>
            {
                await repo.Update(id, updatedCategory);
                return Results.Ok();
            });
            app.MapDelete("/categories/{id:int}", async (int id, ICrudRepository<Category> repo) =>
            {
                await repo.Delete(id);
                return Results.NoContent();
            });

            return app;
        }
    }
}

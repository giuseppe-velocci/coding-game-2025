using Core;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace ProductService.Categories
{
    public static class ProductApiRouting
    {
        public static IEndpointRouteBuilder MapCategoryEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapPost("/categories", async (
                [FromBody] Category category,
                ICrudHandler<Category> handler,
                CancellationToken cts) =>
                {
                    var result = await handler.Create(category, cts);
                    return result.Accept(new CreatedHttpResponseResultVisitor<long>("/categories"));
                });

            app.MapGet("/categories", async (
                ICrudHandler<Category> handler,
                CancellationToken cts) => {
                    var result = await handler.ReadAll(cts);
                    return result.Accept(new HttpResponseResultVisitor<Category[]>());
                });

            app.MapGet("/categories/{id:long}", async (
                long id,
                ICrudHandler<Category> handler,
                CancellationToken cts) =>
            {
                var result = await handler.ReadOne(id, cts);
                return result.Accept(new HttpResponseResultVisitor<Category>());
            });

            app.MapPut("/categories/{id:long}", async (
                long id,
                [FromBody] Category updatedCategory,
                ICrudHandler<Category> handler,
                CancellationToken cts) =>
            {
                var result = await handler.Update(id, updatedCategory, cts);
                return result.Accept(new NoContentHttpResponseResultVisitor<None>());
            });

            app.MapDelete("/categories/{id:int}", async (
                int id,
                ICrudHandler<Category> handler,
                CancellationToken cts) =>
            {
                var result = await handler.Delete(id, cts);
                return result.Accept(new NoContentHttpResponseResultVisitor<None>());
            });

            return app;
        }
    }
}

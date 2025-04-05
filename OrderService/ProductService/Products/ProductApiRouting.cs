using Core;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace ProductService.Products
{
    public static class ProductApiRouting
    {
        public static IEndpointRouteBuilder MapProductEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapPost("/products", async (
                [FromBody] Product Product,
                ICrudHandler<Product> handler,
                CancellationToken cts) =>
                {
                    var result = await handler.Create(Product, cts);
                    return result.Accept(new CreatedHttpResponseResultVisitor<long>("/products"));
                });

            app.MapGet("/products", async (
                ICrudHandler<Product> handler,
                CancellationToken cts) =>
            {
                var result = await handler.ReadAll(cts);
                return result.Accept(new HttpResponseResultVisitor<Product[]>());
            });

            app.MapGet("/products/{id:long}", async (long id, ICrudHandler<Product> handler, CancellationToken cts) =>
            {
                var result = await handler.ReadOne(id, cts);
                return result.Accept(new HttpResponseResultVisitor<Product>());
            });

            app.MapPut("/products/{id:long}", async (
                long id,
                [FromBody] Product updatedCategory,
                ICrudHandler<Product> handler,
                CancellationToken cts) =>
            {
                var result = await handler.Update(id, updatedCategory, cts);
                return result.Accept(new NoContentHttpResponseResultVisitor<None>());
            });

            app.MapDelete("/products/{id:int}", async (
                int id,
                ICrudHandler<Product> handler,
                CancellationToken cts) =>
            {
                var result = await handler.Delete(id, cts);
                return result.Accept(new NoContentHttpResponseResultVisitor<None>());
            });

            return app;
        }
    }
}

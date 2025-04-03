using Core;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace OrderApiGate.Products.Routing
{
    public static class ProductApiRouting
    {
        private const string path = "/products";

        public static IEndpointRouteBuilder MapProductEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapPost(path, async (
                [FromBody] WriteProduct category,
                ICrudHandler<Product, WriteProduct> handler,
                CancellationToken cts) =>
                {
                    var result = await handler.Create(category, cts);
                    return result.Accept(new CreatedHttpResponseResultVisitor<long>(path));
                });

            app.MapGet(path, async (
                ICrudHandler<Product, WriteProduct> handler,
                CancellationToken cts) =>
            {
                var result = await handler.ReadAll(cts);
                return result.Accept(new HttpResponseResultVisitor<Product[]>());
            });

            app.MapGet($"{path}/{{id:long}}", async (
                long id,
                ICrudHandler<Product, WriteProduct> handler,
                CancellationToken cts) =>
            {
                var result = await handler.ReadOne(id, cts);
                return result.Accept(new HttpResponseResultVisitor<Product>());
            });

            app.MapPut($"{path}/{{id:long}}", async (
                long id,
                [FromBody] WriteProduct updatedCategory,
                ICrudHandler<Product, WriteProduct> handler,
                CancellationToken cts) =>
            {
                var result = await handler.Update(id, updatedCategory, cts);
                return result.Accept(new NoContentHttpResponseResultVisitor<None>());
            });

            app.MapDelete($"{path}/{{id:long}}", async (
                long id,
                ICrudHandler<Product, WriteProduct> handler,
                CancellationToken cts) =>
            {
                var result = await handler.Delete(id, cts);
                return result.Accept(new NoContentHttpResponseResultVisitor<None>());
            });

            return app;
        }
    }
}

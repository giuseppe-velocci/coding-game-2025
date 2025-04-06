using Core;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace OrderApiGate.Orders.Routing
{
    public static class OrderApiRouting
    {
        private const string path = "/orders";

        public static IEndpointRouteBuilder MapOrderEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapPost(path, async (
                [FromBody] WriteOrder category,
                ICrudHandler<Order, WriteOrder> handler,
                CancellationToken cts) =>
                {
                    var result = await handler.Create(category, cts);
                    return result.Accept(new CreatedHttpResponseResultVisitor<long>(path));
                });

            app.MapGet(path, async (
                ICrudHandler<Order, WriteOrder> handler,
                CancellationToken cts) =>
            {
                var result = await handler.ReadAll(cts);
                return result.Accept(new HttpResponseResultVisitor<Order[]>());
            });

            app.MapGet($"{path}/{{id:long}}", async (
                long id,
                ICrudHandler<Order, WriteOrder> handler,
                CancellationToken cts) =>
            {
                var result = await handler.ReadOne(id, cts);
                return result.Accept(new HttpResponseResultVisitor<Order>());
            });

            app.MapPut($"{path}/{{id:long}}", async (
                long id,
                [FromBody] WriteOrder updatedCategory,
                ICrudHandler<Order, WriteOrder> handler,
                CancellationToken cts) =>
            {
                var result = await handler.Update(id, updatedCategory, cts);
                return result.Accept(new NoContentHttpResponseResultVisitor<None>());
            });

            app.MapDelete($"{path}/{{id:long}}", async (
                long id,
                ICrudHandler<Order, WriteOrder> handler,
                CancellationToken cts) =>
            {
                var result = await handler.Delete(id, cts);
                return result.Accept(new NoContentHttpResponseResultVisitor<None>());
            });

            return app;
        }
    }
}

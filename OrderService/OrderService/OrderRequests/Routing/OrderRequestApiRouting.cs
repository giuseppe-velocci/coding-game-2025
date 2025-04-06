using Core;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;
using OrderService.OrderRequests.Service;
using OrderService.Orders;

namespace OrderService.OrderRequests.Routing
{
    public static class OrderRequestApiRouting
    {
        private const string path = "/order";

        public static IEndpointRouteBuilder MapOrderRequestEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapPost(path, async (
                [FromBody] OrderRequest OrderRequest,
                IOrderRequestHandler handler,
                CancellationToken cts) =>
                {
                    var result = await handler.Create(OrderRequest, cts);
                    return result.Accept(new CreatedHttpResponseResultVisitor<long>(path));
                });

            app.MapGet(path, async (
                IOrderRequestHandler handler,
                CancellationToken cts) =>
            {
                var result = await handler.ReadAll(cts);
                return result.Accept(new HttpResponseResultVisitor<Order[]>());
            });

            app.MapGet($"{path}/{{id:long}}", async (long id, IOrderRequestHandler handler, CancellationToken cts) =>
            {
                var result = await handler.ReadOne(id, cts);
                return result.Accept(new HttpResponseResultVisitor<Order>());
            });

            app.MapPut($"{path}/{{id:long}}", async (
                long id,
                [FromBody] OrderRequest updatedCategory,
                IOrderRequestHandler handler,
                CancellationToken cts) =>
            {
                var result = await handler.Update(id, updatedCategory, cts);
                return result.Accept(new NoContentHttpResponseResultVisitor<None>());
            });

            app.MapDelete($"{path}/{{id:long}}", async (
                int id,
                IOrderRequestHandler handler,
                CancellationToken cts) =>
            {
                var result = await handler.Delete(id, cts);
                return result.Accept(new NoContentHttpResponseResultVisitor<None>());
            });

            return app;
        }
    }
}

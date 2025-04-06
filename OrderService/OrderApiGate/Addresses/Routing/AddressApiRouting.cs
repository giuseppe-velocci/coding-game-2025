using Core;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace OrderApiGate.Addresses.Routing
{
    public static class AddressApiRouting
    {
        private const string path = "/addresses";

        public static IEndpointRouteBuilder MapAddressEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapPost(path, async (
                [FromBody] WriteAddress category,
                ICrudHandler<Address, WriteAddress> handler,
                CancellationToken cts) =>
                {
                    var result = await handler.Create(category, cts);
                    return result.Accept(new CreatedHttpResponseResultVisitor<long>(path));
                });

            app.MapGet(path, async (
                ICrudHandler<Address, WriteAddress> handler,
                CancellationToken cts) =>
            {
                var result = await handler.ReadAll(cts);
                return result.Accept(new HttpResponseResultVisitor<Address[]>());
            });

            app.MapGet($"{path}/{{id:long}}", async (
                long id,
                ICrudHandler<Address, WriteAddress> handler,
                CancellationToken cts) =>
            {
                var result = await handler.ReadOne(id, cts);
                return result.Accept(new HttpResponseResultVisitor<Address>());
            });

            app.MapPut($"{path}/{{id:long}}", async (
                long id,
                [FromBody] WriteAddress updatedCategory,
                ICrudHandler<Address, WriteAddress> handler,
                CancellationToken cts) =>
            {
                var result = await handler.Update(id, updatedCategory, cts);
                return result.Accept(new NoContentHttpResponseResultVisitor<None>());
            });

            app.MapDelete($"{path}/{{id:long}}", async (
                long id,
                ICrudHandler<Address, WriteAddress> handler,
                CancellationToken cts) =>
            {
                var result = await handler.Delete(id, cts);
                return result.Accept(new NoContentHttpResponseResultVisitor<None>());
            });

            return app;
        }
    }
}

using Core;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;
using UserService.AddressReferences;

namespace UserService.AddressReferences.Routing
{
    public static class AddressReferenceApiRouting
    {
        private const string path = $"/address-references";

        public static IEndpointRouteBuilder MapAddressReferenceEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapPost(path, async (
                [FromBody] AddressReference AddressReference,
                ICrudHandler<AddressReference> handler,
                CancellationToken cts) =>
                {
                    var result = await handler.Create(AddressReference, cts);
                    return result.Accept(new CreatedHttpResponseResultVisitor<long>(path));
                });

            app.MapGet(path, async (
                ICrudHandler<AddressReference> handler,
                CancellationToken cts) =>
            {
                var result = await handler.ReadAll(cts);
                return result.Accept(new HttpResponseResultVisitor<AddressReference[]>());
            });

            app.MapGet($"{path}/{{id:long}}", async (long id, ICrudHandler<AddressReference> handler, CancellationToken cts) =>
            {
                var result = await handler.ReadOne(id, cts);
                return result.Accept(new HttpResponseResultVisitor<AddressReference>());
            });

            app.MapDelete($"{path}/{{id:long}}", async (
                long id,
                ICrudHandler<AddressReference> handler,
                CancellationToken cts) =>
            {
                var result = await handler.Delete(id, cts);
                return result.Accept(new NoContentHttpResponseResultVisitor<None>());
            });

            return app;
        }
    }
}

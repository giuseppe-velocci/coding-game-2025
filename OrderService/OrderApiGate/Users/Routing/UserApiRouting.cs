using Core;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace OrderApiGate.Users.Routing
{
    public static class UserApiRouting
    {
        private const string path = "/users";

        public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapPost(path, async (
                [FromBody] WriteUser category,
                ICrudHandler<User, WriteUser> handler,
                CancellationToken cts) =>
                {
                    var result = await handler.Create(category, cts);
                    return result.Accept(new CreatedHttpResponseResultVisitor<long>(path));
                });

            app.MapGet(path, async (
                ICrudHandler<User, WriteUser> handler,
                CancellationToken cts) =>
            {
                var result = await handler.ReadAll(cts);
                return result.Accept(new HttpResponseResultVisitor<User[]>());
            });

            app.MapGet($"{path}/{{id:long}}", async (
                long id,
                ICrudHandler<User, WriteUser> handler,
                CancellationToken cts) =>
            {
                var result = await handler.ReadOne(id, cts);
                return result.Accept(new HttpResponseResultVisitor<User>());
            });

            app.MapPut($"{path}/{{id:long}}", async (
                long id,
                [FromBody] WriteUser updatedCategory,
                ICrudHandler<User, WriteUser> handler,
                CancellationToken cts) =>
            {
                var result = await handler.Update(id, updatedCategory, cts);
                return result.Accept(new NoContentHttpResponseResultVisitor<None>());
            });

            app.MapDelete($"{path}/{{id:long}}", async (
                long id,
                ICrudHandler<User, WriteUser> handler,
                CancellationToken cts) =>
            {
                var result = await handler.Delete(id, cts);
                return result.Accept(new NoContentHttpResponseResultVisitor<None>());
            });

            return app;
        }
    }
}

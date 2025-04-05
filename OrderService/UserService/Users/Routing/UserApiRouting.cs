using Core;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace UserService.Users.Routing
{
    public static class UserApiRouting
    {
        public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapPost("/users", async (
                [FromBody] User User,
                ICrudHandler<User> handler,
                CancellationToken cts) =>
                {
                    var result = await handler.Create(User, cts);
                    return result.Accept(new CreatedHttpResponseResultVisitor<long>("/users"));
                });

            app.MapGet("/users", async (
                ICrudHandler<User> handler,
                CancellationToken cts) =>
            {
                var result = await handler.ReadAll(cts);
                return result.Accept(new HttpResponseResultVisitor<User[]>());
            });

            app.MapGet("/users/{id:long}", async (long id, ICrudHandler<User> handler, CancellationToken cts) =>
            {
                var result = await handler.ReadOne(id, cts);
                return result.Accept(new HttpResponseResultVisitor<User>());
            });

            app.MapPut("/users/{id:long}", async (
                long id,
                [FromBody] User updatedCategory,
                ICrudHandler<User> handler,
                CancellationToken cts) =>
            {
                var result = await handler.Update(id, updatedCategory, cts);
                return result.Accept(new NoContentHttpResponseResultVisitor<None>());
            });

            app.MapDelete("/users/{id:int}", async (
                int id,
                ICrudHandler<User> handler,
                CancellationToken cts) =>
            {
                var result = await handler.Delete(id, cts);
                return result.Accept(new NoContentHttpResponseResultVisitor<None>());
            });

            return app;
        }
    }
}

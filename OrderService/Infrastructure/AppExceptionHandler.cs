using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Infrastructure
{
    public static class AppExceptionHandler
    {
        public static void RunWithExceptionHandler(this WebApplication app)
        {
            app.UseExceptionHandler(exceptionApp =>
            {
                exceptionApp.Run(async context =>
                {
                    context.Response.ContentType = "application/json";
                    var feature = context.Features.Get<IExceptionHandlerPathFeature>();
                    if (feature?.Error is BadHttpRequestException ex && ex.InnerException is JsonException)
                    {
                        ProblemDetails problemDetails = new()
                        {
                            Title = "Invalid Json",
                            Status = StatusCodes.Status500InternalServerError,
                            Detail = ex.Message
                        };
                        context.Response.StatusCode = 400;
                        await context.Response.WriteAsJsonAsync(problemDetails);
                    }
                    else
                    {
                        context.Response.StatusCode = 500;
                        ProblemDetails problemDetails = new()
                        {
                            Title = "An error occurred during the request processing",
                            Status = StatusCodes.Status500InternalServerError,
                            Detail = feature?.Error.Message ?? "An unexpected error occurred."
                        };
                        await context.Response.WriteAsJsonAsync(problemDetails);
                    }
                });
            });
        }
    }
}

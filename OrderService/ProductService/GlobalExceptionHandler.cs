using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace ProductService
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            ProblemDetails problemDetails = null!;

            if (exception is DbUpdateException)
            {
                problemDetails = new()
                {
                    Title = "An error occurred with the storage",
                    Status = StatusCodes.Status500InternalServerError,
                    Detail = exception.Message
                };
            }
            else if (exception is SqliteException)
            {
                problemDetails = new()
                {
                    Title = "An error occurred with the storage",
                    Status = StatusCodes.Status500InternalServerError,
                    Detail = exception.Message
                };
            }
            else
            {
                problemDetails = new()
                {
                    Title = "An error occurred during the request processing",
                    Status = StatusCodes.Status500InternalServerError,
                    Detail = exception.Message
                };
            }

            httpContext.Response.StatusCode = problemDetails.Status.Value;

            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

            return true;
        }
    }
}

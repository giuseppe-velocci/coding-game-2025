using Core;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

internal static class ApiHandlerHelpers
{
    private readonly static JsonSerializerOptions serializerOptions = new() { PropertyNameCaseInsensitive = true };

    public static OperationResult<T?> GetOperationResultResponse<T>(int statusCode, string responseContent)
    {
        if (statusCode >= 500)
        {
            var obj = JsonSerializer.Deserialize<ProblemDetails>(responseContent, serializerOptions);
            throw new Exception(obj!.Detail);
        }
        else if (statusCode > 200)
        {
            var obj = JsonSerializer.Deserialize<ProblemDetails>(responseContent, serializerOptions);

            return statusCode == 404 ?
                new NotFoundResult<T?>(obj.Detail ?? "Not found") :
                new ValidationFailureResult<T?>(obj.Detail ?? "Invalid request");
        }
        else
        {
            var obj = JsonSerializer.Deserialize<T?>(responseContent, serializerOptions);
            return new SuccessResult<T?>(obj);
        }
    }
}
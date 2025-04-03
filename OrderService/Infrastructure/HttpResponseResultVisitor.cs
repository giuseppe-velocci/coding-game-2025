namespace Infrastructure
{
    using Core;
    using Microsoft.AspNetCore.Http;

    public class HttpResponseResultVisitor<TIn> : IOperationResultVisitor<TIn, IResult>
    {
        public virtual IResult Visit(SuccessResult<TIn> successResult)
        {
            return Results.Ok(successResult.Value);
        }

        public IResult Visit(CriticalFailureResult<TIn> criticalFailureResult)
        {
            return Results.Problem(criticalFailureResult.Message, statusCode: 500);
        }

        public IResult Visit(NotFoundResult<TIn> notFoundResult)
        {
            return Results.Problem(notFoundResult.Message, statusCode: 404);
        }

        public IResult Visit(ValidationFailureResult<TIn> validationFailureResult)
        {
            return Results.Problem(validationFailureResult.Message, statusCode: 400);
        }

        public IResult Visit(InvalidRequestResult<TIn> validationFailureResult)
        {
            return Results.Problem(validationFailureResult.Message, statusCode: 400);
        }
    }

    public class CreatedHttpResponseResultVisitor<TIn>(string _getUri) : HttpResponseResultVisitor<TIn>
    {
        public override IResult Visit(SuccessResult<TIn> successResult)
        {
            return Results.Created($"{_getUri}/{successResult.Value}", successResult);
        }
    }
    
    public class AcceptedHttpResponseResultVisitor<TIn>(string _getUri) : HttpResponseResultVisitor<TIn>
    {
        public override IResult Visit(SuccessResult<TIn> successResult)
        {
            return Results.Accepted($"{_getUri}/{successResult.Value}", successResult);
        }
    }

    public class NoContentHttpResponseResultVisitor<TIn> : HttpResponseResultVisitor<TIn>
    {
        public override IResult Visit(SuccessResult<TIn> successResult)
        {
            return Results.NoContent();
        }
    }
}

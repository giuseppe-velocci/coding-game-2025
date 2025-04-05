namespace Core
{
    public interface IOperationResultVisitor<TIn, TOut>
    {
        TOut Visit(SuccessResult<TIn> successResult);
        TOut Visit(CriticalFailureResult<TIn> criticalFailureResult);
        TOut Visit(NotFoundResult<TIn> notFoundResult);
        TOut Visit(ValidationFailureResult<TIn> validationFailureResult);
        TOut Visit(InvalidRequestResult<TIn> validationFailureResult);
    }
}

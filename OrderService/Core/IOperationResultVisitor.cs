namespace Core
{
    public interface IOperationResultVisitor<T>
    {
        int Visit(SuccessResult<T> successResult);
        int Visit(CriticalFailureResult<T> criticalFailureResult);
        int Visit(NotFoundResult<T> notFoundResult);
        int Visit(FailedValidationResult<T> failedValidationResult);
    }
}

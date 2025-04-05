namespace Core
{
    public interface IBaseValidator<T>
    {
        Task<OperationResult<None>> ValidateItemAsync(T value, CancellationToken cts);
    }
}
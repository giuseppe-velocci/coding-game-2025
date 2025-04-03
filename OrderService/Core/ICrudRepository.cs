namespace Core
{
    public interface ICrudRepository<T> : ICrudHandler<T> where T : class
    {}
    
    public interface ICrudHandler<T> where T : class
    {
        Task<OperationResult<long>> Create(T value, CancellationToken cts);
        Task<OperationResult<None>> Delete(long id, CancellationToken cts);
        Task<OperationResult<T[]>> ReadAll(CancellationToken cts);
        Task<OperationResult<T>> ReadOne(long id, CancellationToken cts);
        Task<OperationResult<None>> Update(long id, T value, CancellationToken cts);
    }

    public interface ICrudHandler<TRead, TWrite> where TRead : class where TWrite : class
    {
        Task<OperationResult<long>> Create(TWrite value, CancellationToken cts);
        Task<OperationResult<None>> Delete(long id, CancellationToken cts);
        Task<OperationResult<TRead[]>> ReadAll(CancellationToken cts);
        Task<OperationResult<TRead>> ReadOne(long id, CancellationToken cts);
        Task<OperationResult<None>> Update(long id, TWrite value, CancellationToken cts);
    }
}
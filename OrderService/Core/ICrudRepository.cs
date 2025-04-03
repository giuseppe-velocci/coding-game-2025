namespace Core
{
    public interface ICrudRepository<T> where T : class
    {
        Task<OperationResult<None>> Create(T category);
        Task<OperationResult<None>> Delete(long id);
        Task<OperationResult<T[]>> ReadAll();
        Task<OperationResult<T>> ReadOne(long id);
        Task<OperationResult<None>> Update(long id, T value);
    }
}
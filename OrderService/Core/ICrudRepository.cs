namespace Core
{
    public interface ICrudRepository<T> where T : class
    {
        Task<OperationResult<None>> Create(T category);
        Task<OperationResult<None>> DeleteCategory(int id);
        Task<OperationResult<T[]>> ReadAll();
        Task<OperationResult<T>> ReadOne(int id);
        Task<OperationResult<None>> Update(T value);
    }
}
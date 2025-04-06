namespace Core
{
    public interface ICrudRepositoryWithReadMany<T> : ICrudRepository<T> where T : class
    {
        public Task<OperationResult<T[]>> ReadMany(IEnumerable<long> ids, CancellationToken cts);
    }
}

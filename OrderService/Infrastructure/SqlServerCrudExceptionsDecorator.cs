using Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure
{
    public class SqlServerCrudExceptionsDecorator<T, TRepository>(
        ILogger<TRepository> _logger,
        TRepository _repo
    ) : ICrudRepository<T>
        where T : class
        where TRepository : ICrudRepository<T>
    {
        public Task<OperationResult<long>> Create(T value, CancellationToken cts)
        {
            try
            {
                cts.ThrowIfCancellationRequested();
                return _repo.Create(value, cts);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Unexpected update exception");
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Unexpected invalid operation exception");
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogError(ex, "Operation cancelled exception");
            }

            return Task.FromResult(new CriticalFailureResult<long>("Unexpected exception") as OperationResult<long>);
        }

        public Task<OperationResult<None>> Delete(long id, CancellationToken cts)
        {
            try
            {
                return _repo.Delete(id, cts);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Unexpected update exception");
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Unexpected invalid operation exception");
            }

            return Task.FromResult(new CriticalFailureResult<None>("Unexpected exception") as OperationResult<None>);
        }

        public Task<OperationResult<T[]>> ReadAll(CancellationToken cts)
        {
            try
            {
                return _repo.ReadAll(cts);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Unexpected update exception");
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Unexpected invalid operation exception");
            }

            return Task.FromResult(new CriticalFailureResult<T[]>("Unexpected exception") as OperationResult<T[]>);
        }

        public Task<OperationResult<T>> ReadOne(long id, CancellationToken cts)
        {
            try
            {
                return _repo.ReadOne(id, cts);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Unexpected update exception");
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Unexpected invalid operation exception");
            }

            return Task.FromResult(new CriticalFailureResult<T>("Unexpected exception") as OperationResult<T>);
        }

        public Task<OperationResult<None>> Update(long id, T value, CancellationToken cts)
        {
            try
            {
                return _repo.Update(id, value, cts);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Unexpected update exception");
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Unexpected invalid operation exception");
            }

            return Task.FromResult(new CriticalFailureResult<None>("Unexpected exception") as OperationResult<None>);
        }
    }
}

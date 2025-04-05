using Core;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure
{
    public class SqliteCrudExceptionsDecorator1<T, TRepository>(
        ILogger<TRepository> _logger,
        TRepository _repo
    ) : ICrudRepository<T>
        where T : class
        where TRepository : ICrudRepository<T>
    {
        public Task<OperationResult<long>> Create(T value, CancellationToken cts)
        {
            return Decorate(() => _repo.Create(value, cts), cts);
        }

        public Task<OperationResult<None>> Delete(long id, CancellationToken cts)
        {
            return Decorate(() => _repo.Delete(id, cts), cts);
        }

        public Task<OperationResult<T[]>> ReadAll(CancellationToken cts)
        {
            return Decorate(() => _repo.ReadAll(cts), cts);
        }

        public Task<OperationResult<T>> ReadOne(long id, CancellationToken cts)
        {
            return Decorate(() => _repo.ReadOne(id, cts), cts);
        }

        public Task<OperationResult<None>> Update(long id, T value, CancellationToken cts)
        {
            return Decorate(() => _repo.Update(id, value, cts), cts);
        }

        public Task<OperationResult<TResult>> Decorate<TResult>(
            Func<Task<OperationResult<TResult>>> dbOperation,
            CancellationToken cts)
        {
            try
            {
                cts.ThrowIfCancellationRequested();
                return dbOperation();
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
            catch (SqliteException ex)
            {
                _logger.LogError(ex, "Unexpected db operation exception");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected db operation exception");
            }

            return Task.FromResult(new CriticalFailureResult<TResult>("Unexpected exception") as OperationResult<TResult>);
        }
    }
}

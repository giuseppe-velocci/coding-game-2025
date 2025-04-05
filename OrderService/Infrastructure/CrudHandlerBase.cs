using Core;

namespace Infrastructure
{
    public class CrudHandlerBase<T>(
        IBaseValidator<T> _validator,
        ICrudRepository<T> _repo
    ) where T : class
    {
        public virtual async Task<OperationResult<long>> Create(T value, CancellationToken cts)
        {
            var validation = await _validator.ValidateItemAsync(value, cts);
            if (validation.Success)
            {
                return await _repo.Create(value, cts);
            }
            else
            {
                return new ValidationFailureResult<long>(validation.Message);
            }
        }

        public virtual Task<OperationResult<None>> Delete(long id, CancellationToken cts)
        {
            if (id > 0)
            {
                return _repo.Delete(id, cts);
            }
            else
            {
                return Task.FromResult(new ValidationFailureResult<None>("Id must be greater than 0") as OperationResult<None>);
            }
        }

        public virtual Task<OperationResult<T[]>> ReadAll(CancellationToken cts)
        {
            return _repo.ReadAll(cts);
        }

        public virtual Task<OperationResult<T>> ReadOne(long id, CancellationToken cts)
        {
            if (id > 0)
            {
                return _repo.ReadOne(id, cts);
            }
            else
            {
                return Task.FromResult(new ValidationFailureResult<T>("Id must be greater than 0") as OperationResult<T>);
            }
        }

        public virtual async Task<OperationResult<None>> Update(long id, T value, CancellationToken cts)
        {
            if (id > 0)
            {
                var validation = await _validator.ValidateItemAsync(value, cts);
                return validation.Success ?
                    await _repo.Update(id, value, cts) :
                    new ValidationFailureResult<None>(validation.Message);
            }
            else
            {
                return new ValidationFailureResult<None>("Id must be greater than 0");
            }
        }
    }
}
﻿using Core;

namespace ProductService.Categories
{
    public class CategoryCrudHandler(
        IBaseValidator<Category> _validator,
        ICrudRepository<Category> _repo
    ) : ICrudHandler<Category>
    {
        public async Task<OperationResult<long>> Create(Category value, CancellationToken cts)
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

        public Task<OperationResult<None>> Delete(long id, CancellationToken cts)
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

        public Task<OperationResult<Category[]>> ReadAll(CancellationToken cts)
        {
            return _repo.ReadAll(cts);
        }

        public Task<OperationResult<Category>> ReadOne(long id, CancellationToken cts)
        {
            if (id > 0)
            {
                return _repo.ReadOne(id, cts);
            }
            else
            {
                return Task.FromResult(new ValidationFailureResult<Category>("Id must be greater than 0") as OperationResult<Category>);
            }
        }

        public async Task<OperationResult<None>> Update(long id, Category value, CancellationToken cts)
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

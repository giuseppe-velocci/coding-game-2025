using Core;

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
            return new SuccessResult<long>(1);
        }

        public Task<OperationResult<None>> Delete(long id, CancellationToken cts)
        {
            throw new NotImplementedException();
        }

        public Task<OperationResult<Category[]>> ReadAll(CancellationToken cts)
        {
            throw new NotImplementedException();
        }

        public Task<OperationResult<Category>> ReadOne(long id, CancellationToken cts)
        {
            throw new NotImplementedException();
        }

        public Task<OperationResult<None>> Update(long id, Category value, CancellationToken cts)
        {
            throw new NotImplementedException();
        }
    }
}

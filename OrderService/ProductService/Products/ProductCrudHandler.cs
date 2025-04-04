using Core;

namespace ProductService.Products
{
    public class ProductCrudHandler(
        IBaseValidator<Product> _validator,
        ICrudRepository<Product> _repo
    ) : ICrudHandler<Product>
    {
        public async Task<OperationResult<long>> Create(Product value, CancellationToken cts)
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

        public Task<OperationResult<Product[]>> ReadAll(CancellationToken cts)
        {
            return _repo.ReadAll(cts);
        }

        public Task<OperationResult<Product>> ReadOne(long id, CancellationToken cts)
        {
            if (id > 0)
            {
                return _repo.ReadOne(id, cts);
            }
            else
            {
                return Task.FromResult(new ValidationFailureResult<Product>("Id must be greater than 0") as OperationResult<Product>);
            }
        }

        public async Task<OperationResult<None>> Update(long id, Product value, CancellationToken cts)
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

using Core;
using Microsoft.EntityFrameworkCore;

namespace ProductService.Categories.Storage
{
    public class CategoryRepository(ProductDbContext _context) : ICrudRepository<Category>
    {
        public async Task<OperationResult<long>> Create(Category value, CancellationToken cts)
        {
            await _context.Categories.AddAsync(value, cts);
            await _context.SaveChangesAsync(cts);
            return new SuccessResult<long>(value.CategoryId);
        }

        public async Task<OperationResult<Category>> ReadOne(long id, CancellationToken cts)
        {
            var category = await _context.Categories.FindAsync(id, cts);
            return category == null ?
                    new NotFoundResult<Category>($"Category {id} not found") :
                    new SuccessResult<Category>(category);
        }

        public Task<OperationResult<Category[]>> ReadAll(CancellationToken cts)
        {
            return _context.Categories
                .ToArrayAsync(cts)
                .ContinueWith(x =>
                {
                    OperationResult<Category[]> res = x.IsCompletedSuccessfully ?
                        new SuccessResult<Category[]>(x.Result) :
                        new CriticalFailureResult<Category[]>("Failed reading categories");
                    return res;
                });
        }

        public async Task<OperationResult<None>> Update(long id, Category value, CancellationToken cts)
        {
            var storedValue = await _context.Categories.FindAsync(id, cts);
            if (storedValue == null)
            {
                return new NotFoundResult<None>("Failed reading categories");
            }

            storedValue.Name = value.Name;

            await _context.SaveChangesAsync(cts);
            return new SuccessResult<None>(None.Instance());
        }

        public async Task<OperationResult<None>> Delete(long id, CancellationToken cts)
        {
            var category = await _context.Categories.FindAsync(id, cts);
            if (category == null)
            {
                return new NotFoundResult<None>($"Category {id} Not found");
            }
            else
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync(cts);
            }

            return new SuccessResult<None>(None.Instance());
        }
    }
}

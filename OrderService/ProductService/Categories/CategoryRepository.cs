using Core;
using Microsoft.EntityFrameworkCore;

namespace ProductService.Categories
{
    public class CategoryRepository(ProductDbContext _context) : ICrudRepository<Category>
    {
        public async Task<OperationResult<None>> Create(Category category)
        {
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
            return new SuccessResult<None>(None.Instance());
        }

        public async Task<OperationResult<Category>> ReadOne(long id)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(p => p.CategoryId == id);
            return category == null ?
                    new NotFoundResult<Category>($"Category {id} not found") :
                    new SuccessResult<Category>(category);
        }

        public Task<OperationResult<Category[]>> ReadAll()
        {
            return _context.Categories
                .ToArrayAsync()
                .ContinueWith(x =>
                {
                    OperationResult<Category[]> res = x.IsCompletedSuccessfully ?
                        new SuccessResult<Category[]>(x.Result) :
                        new CriticalFailureResult<Category[]>("Failed reading products");
                    return res;
                });
        }

        public async Task<OperationResult<None>> Update(long id, Category value)
        {
            var storedValue = await _context.Categories.FirstAsync(x => x.CategoryId == id);
            if (storedValue == null)
            {
                return new NotFoundResult<None>("Failed reading products");
            }

            storedValue.Name = value.Name;

            await _context.SaveChangesAsync();
            return new SuccessResult<None>(None.Instance());
        }

        public async Task<OperationResult<None>> Delete(long id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return new NotFoundResult<None>($"Category {id} Not found");
            }
            else
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
            }

            return new SuccessResult<None>(None.Instance());
        }
    }
}

using Core;
using Microsoft.EntityFrameworkCore;

namespace ProductService.Categories.Storage
{
    public class CategoryRepository<TException>(ProductDbContext _context) : ICrudRepository<Category> where TException : Exception
    {
        public async Task<OperationResult<long>> Create(Category value, CancellationToken cts)
        {
            try
            {
                await _context.Categories.AddAsync(value, cts);
                await _context.SaveChangesAsync(cts);
                return new SuccessResult<long>(value.CategoryId);
            }
            catch (DbUpdateException ex)
            {
                return new InvalidRequestResult<long>(ex.Message);
            }
            catch (TException ex)
            {
                return new InvalidRequestResult<long>(ex.Message);
            }
        }

        public async Task<OperationResult<Category>> ReadOne(long id, CancellationToken cts)
        {
            try
            {
                var category = await _context.Categories.FindAsync(id, cts);
                return category == null ?
                        new NotFoundResult<Category>($"Category {id} not found") :
                        new SuccessResult<Category>(category);
            }
            catch (DbUpdateException ex)
            {
                return new InvalidRequestResult<Category>(ex.Message);
            }
            catch (TException ex)
            {
                return new InvalidRequestResult<Category>(ex.Message);
            }
        }

        public async Task<OperationResult<Category[]>> ReadAll(CancellationToken cts)
        {
            try
            {
                var categories = await _context.Categories.ToArrayAsync(cts);
                return new SuccessResult<Category[]>(categories);
            }
            catch (DbUpdateException ex)
            {
                return new InvalidRequestResult<Category[]>(ex.Message);
            }
            catch (TException ex)
            {
                return new InvalidRequestResult<Category[]>(ex.Message);
            }
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

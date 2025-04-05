using Core;
using Microsoft.EntityFrameworkCore;

namespace ProductService.Products.Storage
{
    public class ProductRepository(ProductDbContext _context) : ICrudRepository<Product>
    {
        public async Task<OperationResult<long>> Create(Product value, CancellationToken cts)
        {
            await _context.Products.AddAsync(value, cts);
            await _context.SaveChangesAsync(cts);
            return new SuccessResult<long>(value.ProductId);
        }

        public async Task<OperationResult<Product>> ReadOne(long id, CancellationToken cts)
        {
            var product = await _context.Products.Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.ProductId == id, cts);
            return product == null ?
                    new NotFoundResult<Product>($"Product {id} not found") :
                    new SuccessResult<Product>(product);
        }

        public Task<OperationResult<Product[]>> ReadAll(CancellationToken cts)
        {
            return _context.Products
                .Include(p => p.Category)
                .ToArrayAsync(cts)
                .ContinueWith(x =>
                {
                    OperationResult<Product[]> res = x.IsCompletedSuccessfully ?
                        new SuccessResult<Product[]>(x.Result) :
                        new CriticalFailureResult<Product[]>("Failed reading products");
                    return res;
                });
        }

        public async Task<OperationResult<None>> Update(long id, Product value, CancellationToken cts)
        {
            var storedValue = await _context.Products.FindAsync(id, cts);
            if (storedValue == null)
            {
                return new NotFoundResult<None>("Failed reading products");
            }

            storedValue.Name = value.Name;
            storedValue.CategoryId = value.CategoryId;

            await _context.SaveChangesAsync(cts);
            return new SuccessResult<None>(None.Instance());
        }

        public async Task<OperationResult<None>> Delete(long id, CancellationToken cts)
        {
            var product = await _context.Products.FindAsync(id, cts);
            if (product == null)
            {
                return new NotFoundResult<None>($"Product {id} Not found");
            }
            else
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync(cts);
            }

            return new SuccessResult<None>(None.Instance());
        }
    }
}

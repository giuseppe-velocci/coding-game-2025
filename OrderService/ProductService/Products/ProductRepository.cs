using Core;
using Microsoft.EntityFrameworkCore;

namespace ProductService.Products
{
    public class ProductRepository(ProductDbContext _context) : ICrudRepository<Product>
    {
        public async Task<OperationResult<None>> Create(Product value)
        {
            await _context.Products.AddAsync(value);
            await _context.SaveChangesAsync();
            return new SuccessResult<None>(None.Instance());
        }

        public async Task<OperationResult<Product>> ReadOne(long id)
        {
            var product = await _context.Products.Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.ProductId == id);
            return product == null ?
                    new NotFoundResult<Product>($"Product {id} not found") :
                    new SuccessResult<Product>(product);
        }

        public Task<OperationResult<Product[]>> ReadAll()
        {
            return _context.Products
                .Include(p => p.Category)
                .ToArrayAsync()
                .ContinueWith(x =>
                {
                    OperationResult<Product[]> res = x.IsCompletedSuccessfully ?
                        new SuccessResult<Product[]>(x.Result) :
                        new CriticalFailureResult<Product[]>("Failed reading products");
                    return res;
                });
        }

        public async Task<OperationResult<None>> Update(long id, Product value)
        {
            var storedValue = await _context.Products.FirstAsync(x => x.ProductId == id);
            if (storedValue == null)
            {
                return new NotFoundResult<None>("Failed reading products");
            }

            storedValue.Name = value.Name;
            storedValue.CategoryId = value.CategoryId;

            await _context.SaveChangesAsync();
            return new SuccessResult<None>(None.Instance());
        }

        public async Task<OperationResult<None>> Delete(long id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return new NotFoundResult<None>($"Product {id} Not found");
            }
            else
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }

            return new SuccessResult<None>(None.Instance());
        }
    }
}

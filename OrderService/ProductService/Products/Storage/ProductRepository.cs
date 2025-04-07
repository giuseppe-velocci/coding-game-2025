using Core;
using Microsoft.EntityFrameworkCore;

namespace ProductService.Products.Storage
{
    public class ProductRepository<TException>(ProductDbContext _context) : ICrudRepository<Product> where TException : Exception
    {
        public async Task<OperationResult<long>> Create(Product value, CancellationToken cts)
        {
            try
            {
                await _context.Products.AddAsync(value, cts);
                await _context.SaveChangesAsync(cts);
                return new SuccessResult<long>(value.ProductId);
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

        public async Task<OperationResult<Product>> ReadOne(long id, CancellationToken cts)
        {
            try
            {
                var product = await _context.Products
                    .Include(p => p.Category)
                    .FirstOrDefaultAsync(p => p.ProductId == id, cts);
                return product == null ?
                        new NotFoundResult<Product>($"Product {id} not found") :
                        new SuccessResult<Product>(product);
            }
            catch (DbUpdateException ex)
            {
                return new InvalidRequestResult<Product>(ex.Message);
            }
            catch (TException ex)
            {
                return new InvalidRequestResult<Product>(ex.Message);
            }
        }

        public async Task<OperationResult<Product[]>> ReadAll(CancellationToken cts)
        {
            try
            {
                var products = await _context.Products
                    .Include(p => p.Category)
                    .ToArrayAsync(cts);
                return new SuccessResult<Product[]>(products);
            }
            catch (DbUpdateException ex)
            {
                return new InvalidRequestResult<Product[]>(ex.Message);
            }
            catch (TException ex)
            {
                return new InvalidRequestResult<Product[]>(ex.Message);
            }
        }

        public async Task<OperationResult<None>> Update(long id, Product value, CancellationToken cts)
        {
            try
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
            catch (DbUpdateException ex)
            {
                return new InvalidRequestResult<None>(ex.Message);
            }
            catch (TException ex)
            {
                return new InvalidRequestResult<None>(ex.Message);
            }
        }

        public async Task<OperationResult<None>> Delete(long id, CancellationToken cts)
        {
            try
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
            catch (DbUpdateException ex)
            {
                return new InvalidRequestResult<None>(ex.Message);
            }
            catch (TException ex)
            {
                return new InvalidRequestResult<None>(ex.Message);
            }
        }
    }
}

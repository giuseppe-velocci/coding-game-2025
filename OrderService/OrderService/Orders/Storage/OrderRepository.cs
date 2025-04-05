using Core;
using Microsoft.EntityFrameworkCore;

namespace OrderService.Orders.Storage
{
    public class OrderRepository(OrderDbContext _context) : ICrudRepository<Order>
    {
        public async Task<OperationResult<long>> Create(Order value, CancellationToken cts)
        {
            _context.Entry(value.User).State = EntityState.Unchanged;
            _context.Entry(value.Address).State = EntityState.Unchanged;

            value.IsActive = true;
            await _context.Orders.AddAsync(value, cts);
            await _context.SaveChangesAsync(cts);

            return new SuccessResult<long>(value.OrderId);
        }

        public async Task<OperationResult<Order>> ReadOne(long id, CancellationToken cts)
        {
            var product = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.Address)
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .FirstOrDefaultAsync(o => o.OrderId == id, cts);

            return product == null ?
                    new NotFoundResult<Order>($"Order {id} not found") :
                    new SuccessResult<Order>(product);
        }

        public Task<OperationResult<Order[]>> ReadAll(CancellationToken cts)
        {
            return _context.Orders
                .Include(o => o.User)
                .Include(o => o.Address)
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .ToArrayAsync(cts)
                .ContinueWith(x =>
                {
                    OperationResult<Order[]> res = x.IsCompletedSuccessfully ?
                        new SuccessResult<Order[]>(x.Result) :
                        new CriticalFailureResult<Order[]>("Failed reading products");
                    return res;
                });
        }

        public async Task<OperationResult<None>> Update(long id, Order value, CancellationToken cts)
        {
            var existingOrder = await ReadOne(id, cts);
            if (existingOrder?.Value == null)
            {
                return new NotFoundResult<None>($"Order {id} not found");
            }
            else if (existingOrder.Value.IsActive)
            {
                existingOrder.Value.UserId = value.UserId;
                existingOrder.Value.AddressId = value.AddressId;
                existingOrder.Value.OrderDate = value.OrderDate;

                existingOrder.Value.OrderDetails.Clear();
                foreach (var orderDetail in value.OrderDetails)
                {
                    existingOrder.Value.OrderDetails.Add(orderDetail);
                }

                _context.Orders.Update(existingOrder.Value);
                await _context.SaveChangesAsync(cts);
                return new SuccessResult<None>(None.Instance());
            }
            else
            {
                return new ValidationFailureResult<None>($"Order {id} was cancelled");
            }
        }

        public async Task<OperationResult<None>> Delete(long id, CancellationToken cts)
        {
            var product = await _context.Orders.FindAsync(id, cts);
            if (product == null)
            {
                return new NotFoundResult<None>($"Order {id} Not found");
            }
            else
            {
                product.IsActive = false;
                await _context.SaveChangesAsync(cts);
            }

            return new SuccessResult<None>(None.Instance());
        }
    }
}
}

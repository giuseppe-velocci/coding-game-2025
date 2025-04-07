using Core;
using Microsoft.EntityFrameworkCore;

namespace OrderService.Orders.Storage
{
    public class OrderRepository<TException>(OrderDbContext _context) : ICrudRepository<Order>
        where TException : Exception
    {
        public async Task<OperationResult<long>> Create(Order value, CancellationToken cts)
        {
            try
            {
                value.IsActive = true;
                await _context.Orders.AddAsync(value, cts);
                await _context.SaveChangesAsync(cts);
                return new SuccessResult<long>(value.OrderId);
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

        public async Task<OperationResult<Order>> ReadOne(long id, CancellationToken cts)
        {
            try
            {
                var product = await _context.Orders
                    .Include(o => o.OrderDetails)
                    .FirstOrDefaultAsync(o => o.OrderId == id, cts);

                return product == null ?
                        new NotFoundResult<Order>($"Order {id} not found") :
                        new SuccessResult<Order>(product);
            }
            catch (TException ex)
            {
                return new InvalidRequestResult<Order>(ex.Message);
            }
        }

        public async Task<OperationResult<Order[]>> ReadAll(CancellationToken cts)
        {
            try
            {
                var orders = await _context.Orders
                    .Include(o => o.OrderDetails)
                    .ToArrayAsync(cts);
                return new SuccessResult<Order[]>(orders);
            }
            catch (TException ex)
            {
                return new InvalidRequestResult<Order[]>(ex.Message);
            }
        }

        public async Task<OperationResult<None>> Update(long id, Order value, CancellationToken cts)
        {
            try
            {
                var existingOrder = await ReadOne(id, cts);
                if (existingOrder?.Value == null)
                {
                    return new NotFoundResult<None>($"Order {id} not found");
                }
                else if (existingOrder.Value.IsActive)
                {
                    var entry = _context.Entry(existingOrder.Value);

                    // isActive cannot be modified
                    if (entry.Property(x => x.IsActive).IsModified)
                    {
                        return new ValidationFailureResult<None>($"Order cannot be reset to Active state when cancelled");
                    }

                    // user cannot be modified
                    if (entry.Property(x => x.UserId).IsModified)
                    {
                        return new ValidationFailureResult<None>($"UserId cannot be modified for an Order");
                    }

                    existingOrder.Value.AddressId = value.AddressId;
                    existingOrder.Value.OrderDate = value.OrderDate;

                    // reset all order details
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

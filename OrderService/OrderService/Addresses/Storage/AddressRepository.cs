using Core;
using Microsoft.EntityFrameworkCore;

namespace OrderService.Addresses.Storage
{
    public class AddressRepository(OrderDbContext _context) : ICrudRepository<Address>
    {
        public async Task<OperationResult<None>> Exists(long id, CancellationToken cts)
        {
            var record = await _context.Addresses
                .Where(x => x.AddressId == id)
                .Select(x => x.AddressId)
                .FirstOrDefaultAsync(cts);

            return record > 0 ?
                new SuccessResult<None>(None.Instance()) :
                new NotFoundResult<None>($"Address {id} not found");
        }

        public async Task<OperationResult<long>> Create(Address value, CancellationToken cts)
        {
            value.IsActive = true;
            await _context.Addresses.AddAsync(value, cts);
            await _context.SaveChangesAsync(cts);
            return new SuccessResult<long>(value.AddressId);
        }

        public async Task<OperationResult<Address>> ReadOne(long id, CancellationToken cts)
        {
            var record = await _context.Addresses.FindAsync(id, cts);
            return record == null ?
                    new NotFoundResult<Address>($"Address {id} not found") :
                    new SuccessResult<Address>(record);
        }

        public Task<OperationResult<Address[]>> ReadAll(CancellationToken cts)
        {
            return _context.Addresses
                .ToArrayAsync(cts)
                .ContinueWith(x =>
                {
                    OperationResult<Address[]> res = x.IsCompletedSuccessfully ?
                        new SuccessResult<Address[]>(x.Result) :
                        new CriticalFailureResult<Address[]>("Failed reading adresses");
                    return res;
                });
        }

        public async Task<OperationResult<None>> Update(long id, Address value, CancellationToken cts)
        {
            var storedValue = await _context.Addresses.FindAsync(id, cts);
            if (storedValue == null)
            {
                return new NotFoundResult<None>("Failed reading adresses");
            }

            storedValue.Street = value.Street;
            storedValue.City = value.City;
            storedValue.Country = value.Country;
            storedValue.State = value.State;
            storedValue.ZipCode = value.ZipCode;

            await _context.SaveChangesAsync(cts);
            return new SuccessResult<None>(None.Instance());
        }

        public async Task<OperationResult<None>> Delete(long id, CancellationToken cts)
        {
            var record = await _context.Addresses.FindAsync(id, cts);
            if (record == null)
            {
                return new NotFoundResult<None>($"Address {id} Not found");
            }
            else
            {
                record.IsActive = false;
                await _context.SaveChangesAsync(cts);
            }

            return new SuccessResult<None>(None.Instance());
        }
    }
}

using Core;
using Microsoft.EntityFrameworkCore;

namespace AddressService.Addresses.Storage
{
    public class AddressRepository(AddressDbContext _context) : ICrudRepository<Address>
    {
        public async Task<OperationResult<long>> Create(Address value, CancellationToken cts)
        {
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
                _context.Addresses.Remove(record);
                await _context.SaveChangesAsync(cts);
            }

            return new SuccessResult<None>(None.Instance());
        }
    }
}

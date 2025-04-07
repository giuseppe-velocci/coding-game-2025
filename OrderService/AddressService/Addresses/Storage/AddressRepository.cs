using Core;
using Microsoft.EntityFrameworkCore;

namespace AddressService.Addresses.Storage
{
    public class AddressRepository<TException>(AddressDbContext _context) : ICrudRepository<Address>
        where TException : Exception
    {
        public async Task<OperationResult<long>> Create(Address value, CancellationToken cts)
        {
            try
            {
                value.IsActive = true;
                await _context.Addresses.AddAsync(value, cts);
                await _context.SaveChangesAsync(cts);
            }
            catch (DbUpdateException ex)
            {
                return new InvalidRequestResult<long>(ex.Message);
            }
            catch (TException ex)
            {
                return new InvalidRequestResult<long>(ex.Message);
            }
            return new SuccessResult<long>(value.AddressId);
        }

        public async Task<OperationResult<Address>> ReadOne(long id, CancellationToken cts)
        {
            try
            {
                var record = await _context.Addresses.FindAsync(id, cts);
                return record == null ?
                        new NotFoundResult<Address>($"Address {id} not found") :
                        new SuccessResult<Address>(record);
            }
            catch (TException ex)
            {
                return new InvalidRequestResult<Address>(ex.Message);
            }
        }

        public Task<OperationResult<Address[]>> ReadAll(CancellationToken cts)
        {
            try
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
            catch (TException ex)
            {
                return Task.FromResult(new InvalidRequestResult<Address[]>(ex.Message) as OperationResult<Address[]>);
            }
        }

        public async Task<OperationResult<None>> Update(long id, Address value, CancellationToken cts)
        {
            try
            {
                var storedValue = await _context.Addresses.FindAsync(id, cts);
                if (storedValue == null)
                {
                    return new NotFoundResult<None>("Failed reading adresses");
                }

                if (!storedValue.IsActive)
                {
                    return new ValidationFailureResult<None>("Address was removed");
                }

                storedValue.Street = value.Street;
                storedValue.City = value.City;
                storedValue.Country = value.Country;
                storedValue.State = value.State;
                storedValue.ZipCode = value.ZipCode;

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
                var record = await _context.Addresses.FindAsync(id, cts);
                if (record == null)
                {
                    return new NotFoundResult<None>($"Address {id} Not found");
                }

                if (record.IsActive)
                {
                    record.IsActive = false;
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

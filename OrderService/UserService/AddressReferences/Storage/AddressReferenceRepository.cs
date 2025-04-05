using Core;
using Microsoft.EntityFrameworkCore;
using UserService;
using UserService.AddressReferences;

namespace AddressRefrenceService.AddressRefrences.Storage
{
    public class AddressReferenceRepository(UserDbContext _context) : ICrudRepository<AddressReference>
    {
        public async Task<OperationResult<long>> Create(AddressReference value, CancellationToken cts)
        {
            await _context.AddressReferences.AddAsync(value, cts);
            await _context.SaveChangesAsync(cts);
            return new SuccessResult<long>(value.AddressId);
        }

        public async Task<OperationResult<AddressReference>> ReadOne(long id, CancellationToken cts)
        {
            var user = await _context.AddressReferences
                .FirstOrDefaultAsync(p => p.AddressId == id, cts);
            return user == null ?
                    new NotFoundResult<AddressReference>($"AddressReference {id} not found") :
                    new SuccessResult<AddressReference>(user);
        }

        public Task<OperationResult<AddressReference[]>> ReadAll(CancellationToken cts)
        {
            return _context.AddressReferences
                .ToArrayAsync(cts)
                .ContinueWith(x =>
                {
                    OperationResult<AddressReference[]> res = x.IsCompletedSuccessfully ?
                        new SuccessResult<AddressReference[]>(x.Result) :
                        new CriticalFailureResult<AddressReference[]>("Failed reading addresses");
                    return res;
                });
        }

        public async Task<OperationResult<None>> Update(long id, AddressReference value, CancellationToken cts)
        {
            throw new NotImplementedException();
        }

        public async Task<OperationResult<None>> Delete(long id, CancellationToken cts)
        {
            var user = await _context.AddressReferences.FindAsync(id, cts);
            if (user == null)
            {
                return new NotFoundResult<None>($"AddressRefrence {id} Not found");
            }
            else
            {
                _context.AddressReferences.Remove(user);
                await _context.SaveChangesAsync(cts);
            }

            return new SuccessResult<None>(None.Instance());
        }
    }
}

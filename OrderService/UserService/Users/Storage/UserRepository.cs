using Core;
using Microsoft.EntityFrameworkCore;

namespace UserService.Users.Storage
{
    public class UserRepository(UserDbContext _context) : ICrudRepository<User>
    {
        public async Task<OperationResult<long>> Create(User value, CancellationToken cts)
        {
            value.IsActive = true;
            await _context.Users.AddAsync(value, cts);
            await _context.SaveChangesAsync(cts);
            return new SuccessResult<long>(value.UserId);
        }

        public async Task<OperationResult<User>> ReadOne(long id, CancellationToken cts)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(p => p.UserId == id, cts);
            return user == null ?
                    new NotFoundResult<User>($"User {id} not found") :
                    new SuccessResult<User>(user);
        }

        public Task<OperationResult<User[]>> ReadAll(CancellationToken cts)
        {
            return _context.Users
                .ToArrayAsync(cts)
                .ContinueWith(x =>
                {
                    OperationResult<User[]> res = x.IsCompletedSuccessfully ?
                        new SuccessResult<User[]>(x.Result) :
                        new CriticalFailureResult<User[]>("Failed reading users");
                    return res;
                });
        }

        public async Task<OperationResult<None>> Update(long id, User value, CancellationToken cts)
        {
            var storedValue = await _context.Users.FindAsync(id, cts);
            if (storedValue == null)
            {
                return new NotFoundResult<None>("Failed reading users");
            }

            if (!storedValue.IsActive)
            {
                return new ValidationFailureResult<None>("User was removed");
            }

            storedValue.Name = value.Name;
            storedValue.Email = value.Email;

            await _context.SaveChangesAsync(cts);
            return new SuccessResult<None>(None.Instance());
        }

        public async Task<OperationResult<None>> Delete(long id, CancellationToken cts)
        {
            var user = await _context.Users.FindAsync(id, cts);
            if (user is null)
            {
                return new NotFoundResult<None>($"User {id} Not found");
            }
            
            if (user.IsActive)
            {
                user.IsActive = false;
                await _context.SaveChangesAsync(cts);
            }

            return new SuccessResult<None>(None.Instance());
        }
    }
}

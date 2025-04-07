using Core;
using Microsoft.EntityFrameworkCore;

namespace UserService.Users.Storage
{
    public class UserRepository<TException>(UserDbContext _context) : ICrudRepository<User> where TException : Exception
    {
        public async Task<OperationResult<long>> Create(User value, CancellationToken cts)
        {
            try
            {
                value.IsActive = true;
                await _context.Users.AddAsync(value, cts);
                await _context.SaveChangesAsync(cts);
                return new SuccessResult<long>(value.UserId);
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

        public async Task<OperationResult<User>> ReadOne(long id, CancellationToken cts)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(p => p.UserId == id, cts);
            return user == null ?
                    new NotFoundResult<User>($"User {id} not found") :
                    new SuccessResult<User>(user);
        }

        public async Task<OperationResult<User[]>> ReadAll(CancellationToken cts)
        {
            try
            {
                var users = await _context.Users.ToArrayAsync();
                return new SuccessResult<User[]>(users);
            }
            catch (DbUpdateException ex)
            {
                return new InvalidRequestResult<User[]>(ex.Message);
            }
            catch (TException ex)
            {
                return new InvalidRequestResult<User[]>(ex.Message);
            }
        }

        public async Task<OperationResult<None>> Update(long id, User value, CancellationToken cts)
        {
            try
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

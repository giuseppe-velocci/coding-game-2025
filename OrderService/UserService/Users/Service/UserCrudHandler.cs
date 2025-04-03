using Core;
using Infrastructure;

namespace UserService.Users.Service
{
    public class UserCrudHandler(
        IBaseValidator<User> _validator,
        ICrudRepository<User> _repo
    ) : CrudHandlerBase<User>(_validator, _repo), ICrudHandler<User>
    { }
}

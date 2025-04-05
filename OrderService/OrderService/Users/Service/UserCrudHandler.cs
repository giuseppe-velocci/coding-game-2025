using Core;
using Infrastructure;

namespace OrderService.Users.Service
{
    public class UserCrudHandler(
        IBaseValidator<User> _validator,
        ICrudRepository<User> _repo
    ) : CrudHandlerBase<User>(_validator, _repo), ICrudHandler<User>
    { }
}

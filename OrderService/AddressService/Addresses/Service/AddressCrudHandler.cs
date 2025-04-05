using AddressService.Addresses;
using Core;
using Infrastructure;

namespace AddressService.Addresses.Service
{
    public class AddressCrudHandler(
        IBaseValidator<Address> _validator,
        ICrudRepository<Address> _repo
    ) : CrudHandlerBase<Address>(_validator, _repo), ICrudHandler<Address>
    { }
}

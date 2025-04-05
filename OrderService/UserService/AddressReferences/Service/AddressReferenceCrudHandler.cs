using Core;
using Infrastructure;
using UserService.AddressReferences;

namespace AddressReferenceService.AddressReferences.Service
{
    public class AddressReferenceCrudHandler(
        IBaseValidator<AddressReference> _validator,
        ICrudRepository<AddressReference> _repo
    ) : CrudHandlerBase<AddressReference>(_validator, _repo), ICrudHandler<AddressReference>
    {
        public override async Task<OperationResult<None>> Update(long id, AddressReference value, CancellationToken cts)
        {
            throw new NotImplementedException();
        }
    }
}

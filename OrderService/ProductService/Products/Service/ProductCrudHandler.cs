using Core;
using Infrastructure;

namespace ProductService.Products.Service
{
    public class ProductCrudHandler(
        IBaseValidator<Product> _validator,
        ICrudRepository<Product> _repo
    ) : CrudHandlerBase<Product>(_validator, _repo), ICrudHandler<Product>
    { }
}

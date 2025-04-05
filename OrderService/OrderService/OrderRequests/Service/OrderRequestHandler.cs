using Core;
using OrderService.Addresses;
using OrderService.Orders;
using OrderService.Products;
using OrderService.Users;

namespace OrderService.OrderRequests.Service
{
    public class OrderRequestHandler(
        IBaseValidator<OrderRequest> _validator,
        ICrudRepository<Order> _orderRepo,
        ICrudRepository<User> _userRepo,
        ICrudRepository<Product> _productRepo,
        ICrudRepository<Address> _addressRepo
    ) : ICrudHandler<OrderRequest>
    {
        public async Task<OperationResult<long>> Create(OrderRequest value, CancellationToken cts)
        {
            var validation = await _validator.ValidateItemAsync(value, cts);
            if (validation.Success)
            {
                // need to gather data from the various storages
                var user = _userRepo.ReadOne(value.UserId, cts);
                var address = 
                    _addressRepo.ReadOne(value.AddressId, cts),
                };
                await Task.WhenAll(userIntegrityChecks);
                Task[] productIntegrityChecks = value.ProductIds
                    .Select(x => _productRepo.ReadOne(x.ProductId, cts))
                    .ToArray();
                return await _orderRepo.Create(value, cts);
            }
            else
            {
                return new ValidationFailureResult<long>(validation.Message);
            }
        }

        public Task<OperationResult<None>> Delete(long id, CancellationToken cts)
        {
            throw new NotImplementedException();
        }

        public Task<OperationResult<OrderRequest[]>> ReadAll(CancellationToken cts)
        {
            throw new NotImplementedException();
        }

        public Task<OperationResult<OrderRequest>> ReadOne(long id, CancellationToken cts)
        {
            throw new NotImplementedException();
        }

        public Task<OperationResult<None>> Update(long id, OrderRequest value, CancellationToken cts)
        {
            throw new NotImplementedException();
        }
    }
}

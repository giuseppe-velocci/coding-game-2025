using Core;
using OrderService.Addresses;
using OrderService.Orders;
using OrderService.Products;
using OrderService.Users;
using System.Text;

namespace OrderService.OrderRequests.Service
{
    public class OrderRequestHandler(
        IBaseValidator<OrderRequest> _validator,
        ICrudRepository<Order> _orderRepo,
        ICrudRepository<User> _userRepo,
        ICrudRepositoryWithReadMany<Product> _productRepo,
        ICrudRepository<Address> _addressRepo,
        IQueueWriter<EventMessage> _queueWriter
    ) : ICrudHandler<OrderRequest>
    {
        public async Task<OperationResult<long>> Create(OrderRequest value, CancellationToken cts)
        {
            var validation = await _validator.ValidateItemAsync(value, cts);
            if (!validation.Success)
            {
                return new ValidationFailureResult<long>(validation.Message);
            }

            // need to gather data from the various local storages
            var productIds = value.ProductIds.Select(x => x.ProductId).ToArray();
            var user = _userRepo.ReadOne(value.UserId, cts);
            var address = _addressRepo.ReadOne(value.AddressId, cts);
            var products = _productRepo.ReadMany(productIds, cts);

            await Task.WhenAll(user, address, products);
            if (RequestIsValid(value, user, address, products))
            {
                return await CreateOrder(_orderRepo, value, products, cts);
            }
            else
            {
                StringBuilder errorsSb = new();

                if (!user.Result.Success && user.Result is NotFoundResult<User>)
                {
                    await SendUserNotFoundEvent(_queueWriter, value, errorsSb, cts);
                }

                if (!address.Result.Success && address.Result is NotFoundResult<Address>)
                {
                    await SendAddressNotFoundEvent(_queueWriter, value, errorsSb, cts);
                }

                if (products.Result.Value is not null && products.Result.Value.Length < value.ProductIds.Count)
                {
                    await SendProductsNotFoundEvent(_queueWriter, productIds, products, errorsSb, cts);
                }

                // retry policy will allow the client to try again
                return new InvalidRequestResult<long>($"Some data in the request was not found: {errorsSb}");
            }

            static bool RequestIsValid(
                OrderRequest value,
                Task<OperationResult<User>> user,
                Task<OperationResult<Address>> address,
                Task<OperationResult<Product[]>> products)
            {
                return user.Result.Success
                    && address.Result.Success
                    && products.Result.Success
                    && products.Result.Value?.Length == value.ProductIds.Count;
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

        private static async Task SendProductsNotFoundEvent(IQueueWriter<EventMessage> _queueWriter, long[] productIds, Task<OperationResult<Product[]>> products, StringBuilder errorsSb, CancellationToken cts)
        {
            var missingProducts = CompareArrays(
                products.Result.Value?.Select(x => x.ProductId).ToArray() ?? [],
                productIds);
            foreach (var productId in missingProducts)
            {
                await _queueWriter.Send(EventMessage.ProductNotFound(productId), cts);
                errorsSb.Append($"ProductId {productId} ");
            }

            static long[] CompareArrays(long[] first, long[] second)
            {
                return second.Except(first).ToArray();
            }
        }

        private static async Task SendAddressNotFoundEvent(IQueueWriter<EventMessage> _queueWriter, OrderRequest value, StringBuilder errorsSb, CancellationToken cts)
        {
            await _queueWriter.Send(EventMessage.AddressNotFound(value.AddressId), cts);
            errorsSb.Append($"AddressId {value.AddressId} ");
        }

        private static async Task SendUserNotFoundEvent(IQueueWriter<EventMessage> _queueWriter, OrderRequest value, StringBuilder errorsSb, CancellationToken cts)
        {
            await _queueWriter.Send(EventMessage.UserNotFound(value.UserId), cts);
            errorsSb.Append($"UserId {value.UserId} ");
        }

        private static async Task<OperationResult<long>> CreateOrder(ICrudRepository<Order> _orderRepo, OrderRequest value, Task<OperationResult<Product[]>> products, CancellationToken cts)
        {
            var orderDetails = products.Result.Value!
                .Select((x, i) => new OrderDetail()
                {
                    ProductId = x.ProductId,
                    UnitPrice = x.Price,
                    Quantity = value.ProductIds[i].Quantity
                })
                .ToArray();
            var order = new Order
            {
                AddressId = value.AddressId,
                UserId = value.UserId,
                OrderDate = DateTime.UtcNow,
                OrderDetails = orderDetails
            };

            return await _orderRepo.Create(order, cts);
        }
    }
}

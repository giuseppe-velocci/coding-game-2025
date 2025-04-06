using Core;
using OrderService.OrderRequests.ExternalModels;
using OrderService.Orders;

namespace OrderService.OrderRequests.Service
{
    public class OrderRequestHandler(
        IBaseValidator<OrderRequest> _validator,
        ICrudRepository<Order> _orderRepo,
        IApiGatewayCaller _apicaller
    ) : ICrudHandler<OrderRequest>
    {
        public async Task<OperationResult<long>> Create(OrderRequest value, CancellationToken cts)
        {
            var validation = await _validator.ValidateItemAsync(value, cts);
            if (!validation.Success)
            {
                return new ValidationFailureResult<long>(validation.Message);
            }

            var user = _apicaller.GetUserAsync(value.UserId, cts);
            var address = _apicaller.GetAddressAsync(value.AddressId, cts);
            await Task.WhenAll(user, address);
            if (user is null || address is null)
            {
                return new ValidationFailureResult<long>(user is null ? "User not found" : "Address not found");
            }

            var productIds = value.ProductIds.Select(x => x.ProductId).ToArray();
            var productRequestBatches = ChunkArray(productIds, 4);
            List<Product> fetchedProducts = new();
            foreach (var productBatch in productRequestBatches)
            {
                var run = productBatch.Select(x => _apicaller.GetProductAsync(x, cts));
                await Task.WhenAll(run);
                fetchedProducts.AddRange(run.Select(x => x.Result).Where(x => x is not null)!);
            }

            if (fetchedProducts.Count == value.ProductIds.Count)
            {
                return await CreateOrder(_orderRepo, value, fetchedProducts, cts);
            }
            else
            {
                var missingIds = productIds.Except(fetchedProducts.Select(x => x.ProductId));
                return new ValidationFailureResult<long>($"Invalid products: {string.Join(", ", missingIds)}");
            }
        }

        private static IEnumerable<T[]> ChunkArray<T>(T[] array, int chunkSize)
        {
            for (int i = 0; i < array.Length; i += chunkSize)
            {
                yield return array.Skip(i).Take(chunkSize).ToArray();
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

        private static async Task<OperationResult<long>> CreateOrder(
            ICrudRepository<Order> _orderRepo,
            OrderRequest value,
            IEnumerable<Product> products,
            CancellationToken cts)
        {
            var orderDetails = products
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

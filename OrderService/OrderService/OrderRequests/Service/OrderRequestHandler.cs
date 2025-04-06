using Core;
using OrderService.OrderRequests.ExternalModels;
using OrderService.Orders;

namespace OrderService.OrderRequests.Service
{
    public class OrderRequestHandler(
        IBaseValidator<OrderRequest> _validator,
        ICrudRepository<Order> _orderRepo,
        IApiGatewayCaller _apicaller
    )
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

            long[] productIds = value.ProductIds.Select(x => x.ProductId).ToArray();
            var fetchedProducts = await GetProducts(_apicaller, value, productIds, cts);
            if (fetchedProducts.Count == value.ProductIds.Count)
            {
                var order = CreateOrderObject(value, fetchedProducts);
                return await _orderRepo.Create(order, cts);

            }
            else
            {
                var missingIds = productIds.Except(fetchedProducts.Select(x => x.ProductId));
                return new ValidationFailureResult<long>($"Invalid products: {string.Join(", ", missingIds)}");
            }
        }

        public Task<OperationResult<None>> Delete(long id, CancellationToken cts)
        {
            return _orderRepo.Delete(id, cts);
        }

        public Task<OperationResult<Order[]>> ReadAll(CancellationToken cts)
        {
            return _orderRepo.ReadAll(cts);
        }

        public Task<OperationResult<Order>> ReadOne(long id, CancellationToken cts)
        {
            return _orderRepo.ReadOne(id, cts);
        }

        public async Task<OperationResult<None>> Update(long id, OrderRequest value, CancellationToken cts)
        {
            var validation = await _validator.ValidateItemAsync(value, cts);
            if (!validation.Success)
            {
                return new ValidationFailureResult<None>(validation.Message);
            }

            // skipping user check because it will not be allowed editing the user on order updates
            var address = await _apicaller.GetAddressAsync(value.AddressId, cts);
            if (address is null)
            {
                return new ValidationFailureResult<None>("Address not found");
            }

            long[] productIds = value.ProductIds.Select(x => x.ProductId).ToArray();
            var fetchedProducts = await GetProducts(_apicaller, value, productIds, cts);
            if (fetchedProducts.Count == value.ProductIds.Count)
            {
                var order = CreateOrderObject(value, fetchedProducts);
                return await _orderRepo.Update(id, order, cts);
            }
            else
            {
                var missingIds = productIds.Except(fetchedProducts.Select(x => x.ProductId));
                return new ValidationFailureResult<None>($"Invalid products: {string.Join(", ", missingIds)}");
            }
            
        }

        private static Order CreateOrderObject(OrderRequest value, IEnumerable<Product> products)
        {
            var orderDetails = products
                .Select((x, i) => new OrderDetail()
                {
                    ProductId = x.ProductId,
                    UnitPrice = x.Price,
                    ProductName = x.Name,
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
            return order;
        }

        private static async Task<List<Product>> GetProducts(IApiGatewayCaller _apicaller, OrderRequest value, long[] productIds, CancellationToken cts)
        {
            var productRequestBatches = ChunkArray(productIds, 4);
            List<Product> fetchedProducts = [];
            foreach (var productBatch in productRequestBatches)
            {
                var run = productBatch.Select(x => _apicaller.GetProductAsync(x, cts));
                await Task.WhenAll(run);
                fetchedProducts.AddRange(run.Select(x => x.Result).Where(x => x is not null)!);
            }
            return fetchedProducts;
        }

        private static IEnumerable<T[]> ChunkArray<T>(T[] array, int chunkSize)
        {
            for (int i = 0; i < array.Length; i += chunkSize)
            {
                yield return array.Skip(i).Take(chunkSize).ToArray();
            }
        }
    }
}

using OrderService.OrderRequests.ExternalModels;

namespace OrderService.OrderRequests.Service
{
    public interface IApiGatewayCaller
    {
        Task<Address?> GetAddressAsync(long id, CancellationToken cts);
        Task<Product?> GetProductAsync(long id, CancellationToken cts);
        Task<User?> GetUserAsync(long id, CancellationToken cts);
    }
}
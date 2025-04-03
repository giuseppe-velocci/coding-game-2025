using Core;
using OrderService.OrderRequests.ExternalModels;

namespace OrderService.OrderRequests.Service
{
    public class ApiGatewayCaller(IHttpClientService _httpClient, ApiEndpointConfig _config) : IApiGatewayCaller
    {
        public async Task<Address?> GetAddressAsync(long id, CancellationToken cts)
        {
            var value = await _httpClient.CallEndpointAsync<Address>($"{_config.ApiGatewayEndpoint}/addresses/{id}", cts);
            return value;
        }

        public async Task<Product?> GetProductAsync(long id, CancellationToken cts)
        {
            var value = await _httpClient.CallEndpointAsync<Product>($"{_config.ApiGatewayEndpoint}/products/{id}", cts);
            return value;
        }

        public async Task<User?> GetUserAsync(long id, CancellationToken cts)
        {
            var value = await _httpClient.CallEndpointAsync<User>($"{_config.ApiGatewayEndpoint}/users/{id}", cts);
            return value;
        }
    }
}

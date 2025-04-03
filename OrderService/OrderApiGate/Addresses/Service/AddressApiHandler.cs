using Core;

namespace OrderApiGate.Addresses.Service
{
    public class AddressApiHandler(IHttpClientFactory httpClientFactory, ApiGateConfig config, ILogger<AddressApiHandler> logger)
        : AbstractApiHandler<Address, WriteAddress>(httpClientFactory, logger), ICrudHandler<Address, WriteAddress>
    {
        protected override string GetEndpointUrl() => $"{config.AddressApiEndpoint}/addresses";
    }
}

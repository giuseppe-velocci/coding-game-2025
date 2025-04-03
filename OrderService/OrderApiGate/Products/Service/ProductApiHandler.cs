using Core;

namespace OrderApiGate.Products.Service
{
    public class ProductApiHandler(IHttpClientFactory httpClientFactory, ApiGateConfig config, ILogger<ProductApiHandler> logger)
        : AbstractApiHandler<Product, WriteProduct>(httpClientFactory, logger), ICrudHandler<Product, WriteProduct>
    {
        protected override string GetEndpointUrl() => $"{config.ProductApiEndpoint}/products";
    }
}

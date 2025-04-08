using Core;

namespace OrderApiGate.Categories.Service
{
    public class CategoryApiHandler(IHttpClientFactory httpClientFactory, ApiGateConfig config, ILogger<CategoryApiHandler> logger)
        : AbstractApiHandler<Category, WriteCategory>(httpClientFactory, logger), ICrudHandler<Category, WriteCategory>
    {
        protected override string GetEndpointUrl() => $"{config.ProductApiEndpoint}/categories";
    }
}

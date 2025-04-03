using Core;

namespace OrderApiGate.Orders.Service
{
    public class OrderApiHandler(IHttpClientFactory httpClientFactory, ApiGateConfig config, ILogger<OrderApiHandler> logger)
        : AbstractApiHandler<Order, WriteOrder>(httpClientFactory, logger), ICrudHandler<Order, WriteOrder>
    {
        protected override string GetEndpointUrl() => $"{config.OrderApiEndpoint}/orders";
    }
}

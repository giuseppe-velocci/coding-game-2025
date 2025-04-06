namespace OrderService.OrderRequests.Service
{
    public class ApiEndpointConfig
    {
        public ApiEndpointConfig(string url, string port)
        {
            ApiGatewayEndpoint = string.IsNullOrEmpty(port) ?
                url :
                $"{url}:{port}";
        }

        public string ApiGatewayEndpoint { get; }
    }
}

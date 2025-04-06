namespace OrderApiGate
{
    public class ApiGateConfig
    {
        public string AddressApiEndpoint { get; set; }
        public string UserApiEndpoint { get; set; }
        public string ProductApiEndpoint { get; set; }
        public string OrderApiEndpoint { get; set; }


        public static string BuildUrl(string url, string port)
        {
            return string.IsNullOrEmpty(port) ?
                    url :
                    $"{url}:{port}";
        }
    }
}

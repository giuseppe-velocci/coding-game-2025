namespace Core
{
    public interface IHttpClientService
    {
        Task<string> CallEndpointAsync(string endpointUrl);
    }
}
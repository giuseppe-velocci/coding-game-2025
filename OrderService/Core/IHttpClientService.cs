namespace Core
{
    public interface IHttpClientService
    {
        Task<T?> CallEndpointAsync<T>(string endpointUrl, CancellationToken cts);
    }
}
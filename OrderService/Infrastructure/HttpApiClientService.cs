using Core;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Extensions.Http;
using Polly.Retry;
using System.Text.Json;

namespace Infrastructure
{
    public class HttpApiClientService : IHttpClientService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<HttpApiClientService> _logger;
        private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;

        public HttpApiClientService(IHttpClientFactory httpClientFactory, ILogger<HttpApiClientService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;

            _retryPolicy = HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(
                    retryCount: 3,
                    sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    onRetry: (outcome, timespan, retryAttempt, context) =>
                    {
                        _logger.LogDebug("Retry {retryAttempt} after {timespan} seconds due to {ex}",
                            retryAttempt,
                            timespan.TotalSeconds,
                            outcome.Exception?.Message ?? outcome.Result.StatusCode.ToString());
                    });
        }

        public async Task<T?> CallEndpointAsync<T>(string endpointUrl, CancellationToken cts)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                var response = await _retryPolicy.ExecuteAsync(() =>
                    httpClient.GetAsync(endpointUrl, cts));

                if (response.IsSuccessStatusCode)
                {
                    var res = await response.Content.ReadAsStringAsync(cts);
                    var obj = JsonSerializer.Deserialize<T>(res);
                    return obj;
                }
                else
                {
                    _logger.LogWarning("HTTP call failed with status code: {StatusCode}", response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred during http call");
            }

            return default;
        }
    }
}

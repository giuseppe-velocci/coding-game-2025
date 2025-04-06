using Core;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Extensions.Http;
using Polly.Retry;

namespace Infrastructure
{
    public class HttpClientService : IHttpClientService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<HttpClientService> _logger;
        private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;

        public HttpClientService(IHttpClientFactory httpClientFactory, ILogger<HttpClientService> logger)
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

        public async Task<string> CallEndpointAsync(string endpointUrl)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                var response = await _retryPolicy.ExecuteAsync(() =>
                    httpClient.GetAsync(endpointUrl));

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
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

            return string.Empty;
        }
    }
}

using Core;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Extensions.Http;
using Polly.Retry;

namespace Infrastructure
{
    public abstract class AsbtractHttpApiClientService
    {
        protected readonly IHttpClientFactory _httpClientFactory;
        protected readonly ILogger _logger;
        protected readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;

        public AsbtractHttpApiClientService(IHttpClientFactory httpClientFactory, ILogger logger)
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
    }
}

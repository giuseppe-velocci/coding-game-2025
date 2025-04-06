﻿using Core;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Infrastructure
{
    public class HttpApiClientService : AsbtractHttpApiClientService, IHttpClientService
    {
        private readonly ILogger<HttpApiClientService> _logger;

        public HttpApiClientService(IHttpClientFactory httpClientFactory, ILogger<HttpApiClientService> logger)
            :base(httpClientFactory, logger)
        {
            _logger = logger;
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

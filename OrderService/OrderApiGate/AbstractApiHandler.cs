using Core;
using Infrastructure;
using System.Text.Json;
using static ApiResponseHelper;

namespace OrderApiGate
{
    public abstract class AbstractApiHandler<TRead, TWrite>(
            IHttpClientFactory httpClientFactory,
            ILogger logger
        ) : AsbtractHttpApiClientService(httpClientFactory, logger), ICrudHandler<TRead, TWrite>
        where TRead : class where TWrite : class
    {
        protected abstract string GetEndpointUrl();

        private readonly static JsonSerializerOptions _serializerOptions = new() { PropertyNameCaseInsensitive = true };

        public async Task<OperationResult<long>> Create(TWrite value, CancellationToken cts)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                var response = await _retryPolicy.ExecuteAsync(() =>
                    httpClient.PostAsJsonAsync(GetEndpointUrl(), value, cts));

                var responseContent = await response.Content.ReadAsStringAsync(cts);
                if (response.IsSuccessStatusCode)
                {
                    var res = JsonSerializer.Deserialize<SerializableResult<long>>(responseContent, _serializerOptions);
                    return new SuccessResult<long>(res.Value);
                }
                else
                {
                    return GetOperationResultResponse<long>((int)response.StatusCode, responseContent);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred during http call");
            }

            return new InvalidRequestResult<long>("Unexpected issues");
        }

        public async Task<OperationResult<None>> Delete(long id, CancellationToken cts)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                var response = await _retryPolicy.ExecuteAsync(() =>
                    httpClient.DeleteAsync($"{GetEndpointUrl()}/{id}", cts));

                var responseContent = await response.Content.ReadAsStringAsync(cts);
                var statusCode = (int)response.StatusCode;
                if (statusCode == 204)
                {
                    return new SuccessResult<None>(None.Instance());
                }

                var intResponse = GetOperationResultResponse<string>(statusCode, responseContent);
                if (statusCode == 404)
                {
                    return new NotFoundResult<None>(intResponse.Message);
                }
                else
                {
                    return new ValidationFailureResult<None>(intResponse.Message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred during http call");
            }

            return new InvalidRequestResult<None>("Unexpected issues");
        }

        public async Task<OperationResult<TRead[]>> ReadAll(CancellationToken cts)
        {
            var httpClient = _httpClientFactory.CreateClient();
            var response = await _retryPolicy.ExecuteAsync(() =>
                httpClient.GetAsync(GetEndpointUrl(), cts));

            var responseContent = await response.Content.ReadAsStringAsync(cts);
            var statusCode = (int)response.StatusCode;
            return GetOperationResultResponse<TRead[]>(statusCode, responseContent)!;
        }

        public async Task<OperationResult<TRead>> ReadOne(long id, CancellationToken cts)
        {
            var httpClient = _httpClientFactory.CreateClient();
            var response = await _retryPolicy.ExecuteAsync(() =>
                httpClient.GetAsync($"{GetEndpointUrl()}/{id}", cts));

            var responseContent = await response.Content.ReadAsStringAsync(cts);
            var statusCode = (int)response.StatusCode;
            return GetOperationResultResponse<TRead>(statusCode, responseContent)!;
        }

        public async Task<OperationResult<None>> Update(long id, TWrite value, CancellationToken cts)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                var response = await _retryPolicy.ExecuteAsync(() =>
                    httpClient.PutAsJsonAsync($"{GetEndpointUrl()}/{id}", value, cts));

                var responseContent = await response.Content.ReadAsStringAsync(cts);
                var statusCode = (int)response.StatusCode;
                if (statusCode == 204)
                {
                    return new SuccessResult<None>(None.Instance());
                }

                var intResponse = GetOperationResultResponse<string>(statusCode, responseContent);
                if (statusCode == 404)
                {
                    return new NotFoundResult<None>(intResponse.Message);
                }
                else
                {
                    return new ValidationFailureResult<None>(intResponse.Message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred during http call");
            }

            return new InvalidRequestResult<None>("Unexpected issues");
        }
    }
}
